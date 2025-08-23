using Sirenix.OdinInspector;
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
    private List<IDataPersistence> dataPersisters = new List<IDataPersistence>();

    private string selectedProfileId = "";
    private Coroutine autoSaveCoroutine;

    [ContextMenu("Save Game")]
    void SaveGameContextMenu()
    {
        SaveGame();
    }

    [Button("Save Game")]
    private void SaveGameButton()
    {
        SaveGame();
    }

    [ContextMenu("Load Game")]
    void LoadGameContextMenu()
    {
        LoadGame();
    }

    [Button("Load Game")]
    private void LoadGameButton()
    {
        LoadGame();
    }

    protected override void Awake()
    {
        base.Awake();
        dataPersisters = FindAllDataPersistenceObjects();
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);


        // Start Auto Saving if enabled.
        if (autoSave)
        {
            if (autoSaveCoroutine != null)
            {
                StopCoroutine(autoSaveCoroutine);
            }

            autoSaveCoroutine = StartCoroutine(AutoSave());
        }

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        InitializeSelectedProfileId();
    }

    protected void Start()
    {
        if (loadOnStart)
        {
            LoadGame();
        }
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        // update the profile to use for saving and loading
        this.selectedProfileId = newProfileId;
        // load the game, which will use that profile, updating our game data accordingly
        LoadGame();
    }

    public void DeleteProfileData(string profileId)
    {
        // delete the data for this profile id
        dataHandler.Delete(profileId);
        // initialize the selected profile id
        InitializeSelectedProfileId();
        // reload the game so that our data matches the newly selected profile id
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
        if (disableDataPersistence)
        {
            return;
        }

        this.gameData = dataHandler.Load(selectedProfileId);

        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        // after starting new Game, this.gameData != null.
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;
        }

        foreach (IDataPersistence dataPersister in dataPersisters)
        {
            dataPersister.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        // return right away if data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        // if we don't have any data to save, log a warning here
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }

        foreach (IDataPersistence dataPersister in dataPersisters)
        {
            dataPersister.SaveData(gameData);
        }

        // timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        // save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileId);
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
            SaveGame();
        }
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // FindObjectsofType takes in an optional boolean to include inactive gameobjects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);
            SaveGame();
            Debug.Log("Auto Saved Game");
        }
    }
}
