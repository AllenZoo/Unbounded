using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadRequester : MonoBehaviour
{
    [SerializeField] private SceneField persistentGameplay;
    [SerializeField] private bool loadPersistentGameplay = true;

    [SerializeField] private List<SceneField> scenesToLoad;
    [SerializeField] private List<SceneField> scenesToUnload;

    [SerializeField] private bool showLoadingBar;

    [Tooltip("Whether this object requests a scene load at Start.")]
    [SerializeField] private bool requestOnStart = false;

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
        if (loadPersistentGameplay && persistentGameplay != null) _scenesToLoad.Add(persistentGameplay);

        EventBus<OnSceneLoadRequest>.Call(new OnSceneLoadRequest
        {
            scenesToLoad = _scenesToLoad,
            scenesToUnload = this.scenesToUnload,
            showLoadingBar = this.showLoadingBar
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
