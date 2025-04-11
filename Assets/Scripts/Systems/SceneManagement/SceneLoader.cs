using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    //[SerializeField] private GameObject cameraMain;

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
        List<AsyncOperation> scenesLoading = new List<AsyncOperation> ();
        
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            scenesLoading.Add(SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive));
        }

        float totalProgress = 0;
        for (int i = 0; i < scenesLoading.Count; ++i)
        {
            var sceneLoading = scenesLoading[i];
            while (!sceneLoading.isDone)
            {
                totalProgress += sceneLoading.progress;
                bar.fillAmount = totalProgress / scenesLoading.Count;
                Debug.Log($"Percentage Loaded {totalProgress / scenesLoading.Count * 100} %");
                yield return null;
            }
        }
    }

    public IEnumerator UnloadScenes(List<SceneField> scenesToLoad)
    {
        yield return null;
    }
}
