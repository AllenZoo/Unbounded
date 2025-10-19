//using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// IMPORTANT NOTE: if it appears that a scene can't/isn't being loaded, ensure that it is added to the build since if it isn't, it won't work.
/// </summary>
[Serializable]
public class SceneLoadRequester : MonoBehaviour
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

    public IEnumerator Start()
    {
        yield return StartCoroutine(LoadBootstrapScene());
        if (requestOnStart)
        {
            RequestSceneLoad();
        }
    }

    public void RequestSceneLoad()
    {
        List<SceneField> _scenesToLoad = new List<SceneField>(scenesToLoad);
        List<SceneField> _scenesToUnload = new List<SceneField>(scenesToUnload);
        if (loadPersistentGameplay && persistentGameplay != null) _scenesToLoad.Add(persistentGameplay);

        _scenesToLoad.Add(activeSceneAfterLoad);

        if (unloadCurrentActiveScene)
        {
            SceneField activeScene = new SceneField(SceneManager.GetActiveScene().name);
            _scenesToUnload.Add(activeScene);
        }

        EventBus<OnSceneLoadRequest>.Call(new OnSceneLoadRequest
        {
            scenesToLoad = _scenesToLoad,
            scenesToUnload = _scenesToUnload,
            showLoadingBar = this.showLoadingBar,
            activeSceneToSet = this.activeSceneAfterLoad,
        });
    }

    private IEnumerator LoadBootstrapScene()
    {
        if (!SceneManager.GetSceneByName("Bootstrap").IsValid())
        {
            // If no current Bootstrap Scene Actively Loaded. Load it.
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Additive);
        }
        yield return null;
    }
}
