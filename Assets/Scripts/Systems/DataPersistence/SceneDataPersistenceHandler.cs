using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDataPersistenceHandler : MonoBehaviour, IDataPersistence
{
    private SceneField currentScene;

    private EventBinding<OnSceneLoadRequestFinish> sceneLoadFinishBinding;


    private void Awake()
    {
        sceneLoadFinishBinding = new EventBinding<OnSceneLoadRequestFinish>(FetchCurrentScene);
    }

    private void OnEnable()
    {
        EventBus<OnSceneLoadRequestFinish>.Register(sceneLoadFinishBinding);
    }

    private void OnDisable() 
    { 
        EventBus<OnSceneLoadRequestFinish>.Unregister(sceneLoadFinishBinding);
    }

    private void Start()
    {
        FetchCurrentScene();
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
        data.currentScene = currentScene;
    }


    private void FetchCurrentScene()
    {
        currentScene = new SceneField(SceneManager.GetActiveScene().name);
    }
}
