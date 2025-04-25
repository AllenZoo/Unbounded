using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneLoader : MonoBehaviour
{
    [Tooltip("Temporary Camera to capture the loading screen if other cameras don't exist.")]
    [SerializeField] private GameObject cameraMain;

    [Tooltip("Canvas loading screen that is used to hide old screen during loading process.")]
    [SerializeField] private GameObject loadingCanvasPfb;
    [Tooltip("The image of the loading bar in the loading canvas pfb")]
    [SerializeField] private Image bar;

    private void Awake()
    {
        Assert.IsNotNull(cameraMain);
        Assert.IsNotNull(loadingCanvasPfb);
        Assert.IsNotNull(bar);

        EventBinding<OnSceneLoadRequest> onSceneLoadBinding = new EventBinding<OnSceneLoadRequest>(OnSceneLoadRequestEvent);
        EventBus<OnSceneLoadRequest>.Register(onSceneLoadBinding);
    }

    private void OnEnable()
    {
        HideLoadingScreen();
    }

    private void OnSceneLoadRequestEvent(OnSceneLoadRequest e)
    {
        StartCoroutine(OnSceneLoadRequestEventCoroutine(e));
    }
    private IEnumerator OnSceneLoadRequestEventCoroutine(OnSceneLoadRequest e)
    {
        if (e.showLoadingBar)
        {
            ShowLoadingScreen();
        }
        EventBus<OnPauseChangeRequest>.Call(new OnPauseChangeRequest() { shouldPause = true });
        yield return StartCoroutine(LoadScenes(e.scenesToLoad, e.activeSceneToSet, e.showLoadingBar));
        yield return StartCoroutine(UnloadScenes(e.scenesToUnload));
        EventBus<OnSceneLoadRequestFinish>.Call(new OnSceneLoadRequestFinish());
        yield return new WaitForSecondsRealtime(1f); // Delay a bit so that scene transition more smooth. (if we dont do this, we will see previous frame for a split second before transition). This due to how our pause system affects camera.
        EventBus<OnPauseChangeRequest>.Call(new OnPauseChangeRequest() { shouldPause = false });
        HideLoadingScreen();

        yield return null;
    }
    private IEnumerator LoadScenes(List<SceneField> rawScenesToLoad, SceneField activeSceneToSet, bool showingLoadingBar)
    {
        List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
        HashSet<SceneField> scenesToLoad = FilterScenesToLoad(rawScenesToLoad);
        foreach (SceneField scene in scenesToLoad)
        {
            scenesLoading.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
        }


        float totalProgress = 0;
        bool tweenComplete = false;
        for (int i = 0; i < scenesLoading.Count; ++i)
        {
            var sceneLoading = scenesLoading[i];
            while (!sceneLoading.isDone)
            {
                totalProgress += sceneLoading.progress;
                float targetFill = totalProgress / scenesLoading.Count;

                if (showingLoadingBar) bar.DOFillAmount(targetFill, 0.5f).SetUpdate(true);

                //Debug.Log($"Percentage Loaded {totalProgress / scenesLoading.Count * 100} %");
                yield return null; // NOTE: IMPORTANT DO NOT REMOVE THIS LINE OR IT WILL INFINITE LOOP.
            }
        }

        if (showingLoadingBar)
        {
            bar.DOFillAmount(1f, 0.6f)
               .SetUpdate(true) // uses unscaled time
               .OnComplete(() =>
               {
                   tweenComplete = true;
#if UNITY_EDITOR
                   Debug.Log("Tween completed");
#endif
               });
        }


        // Set active scene if there is one.
        if (activeSceneToSet != "")
        {
            Scene activeScene = SceneManager.GetSceneByName(activeSceneToSet);
            SceneManager.SetActiveScene(activeScene);
        }

        if (showingLoadingBar) yield return new WaitUntil(() => tweenComplete);

        yield return new WaitForSecondsRealtime(0.5f);
    }
    private IEnumerator UnloadScenes(List<SceneField> scenesToUnload)
    {
        if (scenesToUnload == null || scenesToUnload.Count == 0)
        {
            yield break;
        }

        List<AsyncOperation> scenesUnloading = new List<AsyncOperation>(scenesToUnload.Count);

        // Start all scene unloading operations
        foreach (var scene in scenesToUnload)
        {
            // Check if there is a scene to unload.
            if (SceneManager.GetSceneByName(scene).IsValid())
            {
                scenesUnloading.Add(SceneManager.UnloadSceneAsync(scene));
            }

        }

        // Wait for all unloading operations to complete
        foreach (var operation in scenesUnloading)
        {
            yield return new WaitUntil(() => operation.isDone);
        }
    }

    /// <summary>
    /// Given a list of scenes, filters out any invalid or already loaded scenes. Should also remove duplicates.
    /// </summary>
    /// <param name="rawScenesToLoad"></param>
    /// <returns></returns>
    private HashSet<SceneField> FilterScenesToLoad(List<SceneField> rawScenesToLoad)
    {
        HashSet<SceneField> filteredScenes = new HashSet<SceneField>();
        foreach (var rawScene in rawScenesToLoad)
        {
            if (rawScene == "") continue;

            // Check if Scene to load is already loaded. If it is, don't add it to list.
            if (!SceneManager.GetSceneByName(rawScene).IsValid())
            {
                filteredScenes.Add(rawScene);
            }
        }
        return filteredScenes;
    }

    public void ShowLoadingScreen()
    {
        if (loadingCanvasPfb != null)
        {
            cameraMain.SetActive(true);
            loadingCanvasPfb.SetActive(true);
            bar.fillAmount = 0;
        }
    }
    public void HideLoadingScreen()
    {
        if (loadingCanvasPfb != null)
        {
            loadingCanvasPfb.SetActive(false);
            cameraMain.SetActive(false);
        }
    }
}