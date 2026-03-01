using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Listens for OnSceneTeleportRequest events and handles teleporting the player to the requested scene.
/// 
/// Control Flow: Receives OnSceneTeleportRequest -> Sends OnSceneLoadRequest
/// 
/// Holds and updates relevant state information as needed.
/// </summary>
public class SceneTeleportListener : Singleton<SceneTeleportListener>
{
    // Active Scene is only set via OnSceneTeleportRequest events.
    [ReadOnly, SerializeField] private SceneField activeSceneAfterLoad;
    [SerializeField] private SceneField persistentGameplay;
    [SerializeField] private bool loadPersistentGameplay = true;
    [SerializeField] private bool unloadCurrentActiveScene = true;
    [SerializeField] private bool showLoadingBar = true;


    protected override void Awake()
    {
        base.Awake();
        EventBinding<OnSceneTeleportRequest> ostrBinding = new EventBinding<OnSceneTeleportRequest>(OnSceneTeleportRequestHandler);
        EventBus<OnSceneTeleportRequest>.Register(ostrBinding);
    }

    private void OnSceneTeleportRequestHandler(OnSceneTeleportRequest evt)
    {
        SceneField activeScene = new SceneField(SceneManager.GetActiveScene().name);
        SceneField activeSceneToSet = evt.targetScene;

        List<SceneField> _scenesToLoad = new List<SceneField>() { activeSceneToSet };
        List<SceneField> _scenesToUnload = new List<SceneField>() { activeScene };

        bool unloadAllButPersistent = evt.unloadAllButPersistent;

        if (loadPersistentGameplay && persistentGameplay != null) _scenesToLoad.Add(persistentGameplay);

        EventBus<OnSceneLoadRequest>.Call(new OnSceneLoadRequest
        {
            scenesToLoad = _scenesToLoad,
            scenesToUnload = _scenesToUnload,
            showLoadingBar = this.showLoadingBar,
            activeSceneToSet = activeSceneToSet,
            unloadAllButPersistent = unloadAllButPersistent
        });
    }
}
