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
        EventBus<OnSceneTeleportRequest>.Call(new OnSceneTeleportRequest { targetScene = data.currentScene, unloadAllButPersistent = true});
    }

    public void SaveData(GameData data)
    {
        data.currentScene = lastValidActiveScene;
    }


    private void FetchCurrentSceneAndSave()
    {
        currentScene = new SceneField(SceneManager.GetActiveScene().name);

        if (lastValidActiveScene == null) lastValidActiveScene = currentScene;

        string[] invalidActiveScenes = new string[] { "DontDestroyOnLoad", "PersistentScene", "MenuScene", "DebuggingTools" };

        if (!invalidActiveScenes.Contains(currentScene.SceneName))
        {
            lastValidActiveScene = currentScene;
        }

        DataPersistenceHandler.Instance.SaveGame(sync:false);
    }
}
