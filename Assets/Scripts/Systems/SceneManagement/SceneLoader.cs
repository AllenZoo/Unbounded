using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// TODO: add a tweening effect to make loading bar progress smoother
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
        yield return StartCoroutine(LoadScenes(e.scenesToLoad));
        yield return StartCoroutine(UnloadScenes(e.scenesToUnload));
        HideLoadingScreen();
        yield return null;
    }
    private IEnumerator LoadScenes(List<SceneField> scenesToLoad)
    {
        List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            var scene = scenesToLoad[i];
            if (scene == "") continue;

            // Check if Scene to load is already loaded. If it is, don't add it to list.
            if (!SceneManager.GetSceneByName(scene).IsValid())
            {
                scenesLoading.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
            }
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
                bar.DOFillAmount(targetFill, 0.5f);
                //Debug.Log($"Percentage Loaded {totalProgress / scenesLoading.Count * 100} %");
                yield return null; // NOTE: IMPORTANT DO NOT REMOVE THIS LINE OR IT WILL INFINITE LOOP.
            }
        }
        bar.DOFillAmount(1f, 0.6f).OnComplete(()=>tweenComplete = true);
        yield return new WaitUntil(() => tweenComplete);
        yield return new WaitForSeconds(0.5f);
    }
    public IEnumerator UnloadScenes(List<SceneField> scenesToUnload)
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
        if (loadingCanvasPfb != null) { 
            loadingCanvasPfb.SetActive(false); 
            cameraMain.SetActive(false); 
        }
    }
}