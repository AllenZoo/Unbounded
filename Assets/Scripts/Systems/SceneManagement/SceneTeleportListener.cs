using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Listens for OnSceneTeleportRequest events and handles teleporting the player to the requested scene.
/// 
/// Holds and updates relevant state information as needed.
/// </summary>
public class SceneTeleportListener : Singleton<SceneTeleportListener>
{
    [SerializeField] private SceneField activeSceneAfterLoad;

    [SerializeField] private SceneField persistentGameplay;
    [SerializeField] private bool loadPersistentGameplay = true;

    [SerializeField] private List<SceneField> scenesToLoad;
    [SerializeField] private List<SceneField> scenesToUnload;

    [SerializeField] private bool showLoadingBar;

    [Tooltip("Whether this object requests a scene load at Start.")]
    [SerializeField] private bool requestOnStart = false;

    [SerializeField] private bool unloadCurrentActiveScene = false;

    protected override void Awake()
    {
        base.Awake();
        EventBinding<OnSceneTeleportRequest> ostrBinding = new EventBinding<OnSceneTeleportRequest>(OnSceneTeleportRequestHandler);
        EventBus<OnSceneTeleportRequest>.Register(ostrBinding);
    }

    private void OnSceneTeleportRequestHandler(OnSceneTeleportRequest evt)
    {
       
    }
}
