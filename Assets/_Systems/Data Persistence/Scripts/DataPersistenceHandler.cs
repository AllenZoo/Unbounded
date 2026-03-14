using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataPersistenceHandler : Singleton<DataPersistenceHandler>
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [Tooltip("True if we want to override current profile ID with 'testSelectedProfileId'")]
    [SerializeField] private bool overrideSelectedProfileId = false;
    [Tooltip("The test profile Id we can choose to override with.")]
    [SerializeField] private string testSelectedProfileId = "test";
    [SerializeField] private bool loadOnStart = false;
    [SerializeField] private bool saveOnApplicationQuit = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption = true;

    [Header("Auto Saving Configuration")]
    [SerializeField] private bool autoSave = false;
    [SerializeField] private float autoSaveTimeSeconds = 60f;

    private FileDataHandler dataHandler;
    private GameData gameData;
    private readonly HashSet<IDataPersistence> dataPersisters = new();

    private string selectedProfileId = "";
    private Coroutine autoSaveCoroutine;

    private bool isSaving = false;

    private EventBinding<OnLoadGameRequest> loadGameRequestEventBinding;

    #region Registration
    public void Register(IDataPersistence dataPersister)
    {
        dataPersisters.Add(dataPersister);
    }

    public void Unregister(IDataPersistence dataPersister)
    {
        dataPersisters.Remove(dataPersister);
    }
    #endregion

    #region Inspector Buttons

    [Button("Save Game")]
    private void SaveGameButton()
    {
        SaveGame();
    }


    [Button("Load Game")]
    private void LoadGameButton()
    {
        LoadGame();
    }

    [Button("New Game (creates blank GameData)")]
    private void CreateNewGameButton()
    {
        NewGame();
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        InitializeSelectedProfileId();
        NewGame();
        loadGameRequestEventBinding = new EventBinding<OnLoadGameRequest>(LoadGame);
    }

    protected void Start()
    {
        // Start Auto Saving if enabled.
        if (autoSave)
        {
            if (autoSaveCoroutine != null)
            {
                StopCoroutine(autoSaveCoroutine);
            }
            autoSaveCoroutine = StartCoroutine(AutoSave());
        }

        if (loadOnStart)
        {
            LoadGame();
        }

        // Auto Discovery of IDataPersistence objects in the scene and register them
        List<IDataPersistence> dataPersistenceObjects = FindAllDataPersistenceObjects();
        dataPersisters.UnionWith(dataPersistenceObjects);
    }

    protected void OnEnable()
    {
        if (loadGameRequestEventBinding != null)
        {
            EventBus<OnLoadGameRequest>.Register(loadGameRequestEventBinding);
        }
    }

    protected void OnDisable()
    {
        if (loadGameRequestEventBinding != null)
        {
            EventBus<OnLoadGameRequest>.Unregister(loadGameRequestEventBinding);
        }
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        this.selectedProfileId = newProfileId;
        LoadGame();
    }

    public void DeleteProfileData(string profileId)
    {
        dataHandler.Delete(profileId);
        InitializeSelectedProfileId();
        LoadGame();
    }

    private void InitializeSelectedProfileId()
    {
        this.selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
        if (overrideSelectedProfileId)
        {
            this.selectedProfileId = testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedProfileId);
        }
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        if (disableDataPersistence) return;

        this.gameData = dataHandler.Load(selectedProfileId);

        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;
        }

        // Version Check and Migration
        if (gameData.gameVersion != Application.version)
        {
            Debug.LogWarning($"Loaded data version ({gameData.gameVersion}) differs from current version ({Application.version}). Attempting migration.");
            MigrateData(gameData);
            gameData.gameVersion = Application.version;
        }

        // Notify all registered persisters to load data
        foreach (IDataPersistence dataPersister in dataPersisters)
        {
            dataPersister.LoadData(gameData);
        }
    }

    private void MigrateData(GameData data)
    {
        // Add future migration logic here 
        // Example: if (data.gameVersion == "0.1") { ... migrate to 0.2 ... }
    }

    public async void SaveGame(bool sync = true)
    {
        if (disableDataPersistence || isSaving) return;

        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }

        // Notify all registered persisters to save data into gameData object
        foreach (IDataPersistence dataPersister in dataPersisters)
        {
            dataPersister.SaveData(gameData);
        }

        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // 2. Create a "Snapshot" / Deep Copy (MUST BE MAIN THREAD)
        // This ensures the background thread has its own private version to work with (if async mode is selected)
        // Prevents game data from being modified while the background thread is serializing and writing to disk, which could cause errors or data corruption.
        GameData dataSnapshot = (GameData) SerializationUtility.CreateCopy(gameData);

        // 3. Offload Serialization and Disk I/O to a background thread
        string profileId = selectedProfileId;

        try
        {
            isSaving = true;
            Debug.Log("Saving Game Data" + (sync ? " (Sync)" : " (Async)"));
            if (sync)
            {
                dataHandler.Save(dataSnapshot, profileId);
            }
            else
            {
                await System.Threading.Tasks.Task.Run(() => dataHandler.Save(dataSnapshot, profileId));
            }
        } catch (System.Exception e)
        {
            Debug.LogError("Error occurred while saving data: " + e.Message);
        }
        finally
        {
            isSaving = false;
        }

    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    private void OnApplicationQuit()
    {
        if (saveOnApplicationQuit)
        {
            SaveGame(true);
        }
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            SaveGame(sync:false);
            Debug.Log("Auto Saved Game (Async)");
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IDataPersistence>().ToList();
    }
}