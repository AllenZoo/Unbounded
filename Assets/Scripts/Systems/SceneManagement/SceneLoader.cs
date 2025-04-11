using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject camera;

    [Tooltip("Canvas loading screen that is used to hide old screen during loading process.")]
    [SerializeField] private GameObject loadingCanvasPfb;
    [SerializeField] private Image bar;

    private void Awake()
    {
        EventBinding<OnSceneLoadRequest> onSceneLoadBinding = new EventBinding<OnSceneLoadRequest>(OnSceneLoadRequestEvent);
        EventBus<OnSceneLoadRequest>.Register(onSceneLoadBinding);
    }

    private void OnSceneLoadRequestEvent(OnSceneLoadRequest e)
    {
        // TODO: show loading bar only if e.showLoadingBar = true.
        ShowLoadingScreen();
        StartCoroutine(LoadScenes(e.scenesToLoad));
        StartCoroutine(UnloadScenes(e.scenesToUnload));
        //HideLoadingScreen();
    }

    public void ShowLoadingScreen()
    {
        if (loadingCanvasPfb != null) { loadingCanvasPfb.SetActive(true); }
    }

    public void HideLoadingScreen()
    {
        if (loadingCanvasPfb != null) { loadingCanvasPfb.SetActive(false); }
    }

    public IEnumerator LoadScenes(List<SceneField> scenesToLoad)
    {
        StartCoroutine(LoadScenesCoroutine(scenesToLoad));
        yield return null;
    }

    private IEnumerator LoadScenesCoroutine(List<SceneField> scenesToLoad)
    {
        float totalProgress = 0;
        for (int i = 0; i < scenesToLoad.Count; ++i)
        {
            var scene = scenesToLoad[i];

            AsyncOperation op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

            while (!op.isDone)
            {
                // Display progress
                Debug.Log($"Percentage Loaded {op.progress}");
                bar.fillAmount = op.progress;
                yield return null;
            }

        }
    }

    public IEnumerator UnloadScenes(List<SceneField> scenesToLoad)
    {
        yield return null;
    }
}
