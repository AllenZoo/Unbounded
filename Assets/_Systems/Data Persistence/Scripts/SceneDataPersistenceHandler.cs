using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataPersistenceHandler : MonoBehaviour, IDataPersistence
{
    private SceneField currentScene;
    private SceneField lastValidActiveScene;
    private EventBinding<OnSceneLoadRequestFinish> sceneLoadFinishBinding;


    private void Awake()
    {
        sceneLoadFinishBinding = new EventBinding<OnSceneLoadRequestFinish>(FetchCurrentSceneAndSave);
    }

    private void OnEnable()
    {
        EventBus<OnSceneLoadRequestFinish>.Register(sceneLoadFinishBinding);
        if (DataPersistenceHandler.Instance != null) DataPersistenceHandler.Instance.Register(this);
    }

    private void OnDisable()
    {
        EventBus<OnSceneLoadRequestFinish>.Unregister(sceneLoadFinishBinding);
        if (DataPersistenceHandler.Instance != null) DataPersistenceHandler.Instance.Unregister(this);
    }

    private void Start()
    {
        FetchCurrentSceneAndSave();
    }

    public void LoadData(GameData data)
    {
        Debug.Log($"Loading scene: {data.currentScene.SceneName}");
        currentScene = data.currentScene;
        // Load the current scene from the GameData and set it as the active scene
        EventBus<OnSceneTeleportRequest>.Call(new OnSceneTeleportRequest { targetScene = data.currentScene, unloadAllButPersistent = true });
    }

    public void SaveData(GameData data)
    {
        data.currentScene = lastValidActiveScene;
    }

    public void ResetData()
    {
        // Set default scene to armoury
        lastValidActiveScene = new SceneField("Armoury");
        currentScene = lastValidActiveScene;
    }

    private void FetchCurrentSceneAndSave()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        currentScene = new SceneField(currentSceneName);

        string[] invalidActiveScenes = new string[] { "DontDestroyOnLoad", "PersistentScene", "MenuScene", "DebuggingTools" };
        bool isInvalidScene = System.Linq.Enumerable.Contains(invalidActiveScenes, currentSceneName);

        // Update the last valid scene ONLY if we are in a gameplay scene
        if (!isInvalidScene)
        {
            lastValidActiveScene = currentScene;
            // Only save the game if we've successfully transitioned to a valid location
            DataPersistenceHandler.Instance.SaveGame(sync: false);
        }
        else if (lastValidActiveScene == null)
        {
            // If we are in the menu and have no valid scene yet, default to Armoury
            lastValidActiveScene = new SceneField("Armoury");
        }
    }
}
