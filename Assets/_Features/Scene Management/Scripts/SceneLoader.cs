using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

/// <summary>
/// Used in BootStrap scene to manage scene loading.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Tooltip("Temporary Camera to capture the loading screen if other cameras don't exist.")]
    [SerializeField] private GameObject cameraMain;

    [Tooltip("Canvas loading screen that is used to hide old screen during loading process.")]
    [SerializeField] private GameObject loadingCanvasPfb;
    [Tooltip("The image of the loading bar in the loading canvas pfb")]
    [SerializeField] private Image bar;

    [Tooltip("List of scenes that should never be unloaded.")]
    [SerializeField] private List<SceneField> persistentScenes;


    private Queue<OnSceneLoadRequest> loadQueue;
    private List<Camera> disabledCameras = new List<Camera>();
    private bool isLoading = false;
    private Coroutine queueCoroutine;

    private void Awake()
    {
        Assert.IsNotNull(cameraMain);
        Assert.IsNotNull(loadingCanvasPfb);
        Assert.IsNotNull(bar);

        EventBinding<OnSceneLoadRequest> onSceneLoadBinding = new EventBinding<OnSceneLoadRequest>(OnSceneLoadRequestEvent);
        EventBus<OnSceneLoadRequest>.Register(onSceneLoadBinding);

        loadQueue = new Queue<OnSceneLoadRequest>();
    }

    private void OnEnable()
    {
        HideLoadingScreen();
    }

    private void OnSceneLoadRequestEvent(OnSceneLoadRequest e)
    {
        loadQueue.Enqueue(e);
        if (!isLoading)
        {
            if (queueCoroutine != null) StopCoroutine(queueCoroutine);
            queueCoroutine = StartCoroutine(HandleLoadQueue());
        }
    }

    private IEnumerator HandleLoadQueue()
    {
        isLoading = true;
        
        try
        {
            while (loadQueue.Count > 0)
            {
                OnSceneLoadRequest loadRequest = loadQueue.Dequeue();
                yield return StartCoroutine(OnSceneLoadRequestEventCoroutine(loadRequest));
            }
        }
        finally
        {
            HideLoadingScreen();
            isLoading = false;
            queueCoroutine = null;
        }
    }

    private IEnumerator OnSceneLoadRequestEventCoroutine(OnSceneLoadRequest e)
    {
        if (e.showLoadingBar)
        {
            ShowLoadingScreen();
        }

        // Pause the game while loading. We will unpause after loading is done by disposing the token.
        using (PauseToken pt = PauseManager.Instance.RequestPause())
        {
            if (e.unloadAllButPersistent)
            {
                yield return StartCoroutine(UnloadAllExceptPersistent());
            }

            yield return StartCoroutine(UnloadScenes(e.scenesToUnload));
            
            // Perform the loading
            yield return StartCoroutine(LoadScenes(e.scenesToLoad, e.activeSceneToSet, e.showLoadingBar));
            
            EventBus<OnSceneLoadRequestFinish>.Call(new OnSceneLoadRequestFinish() { finishedActiveScene = e.activeSceneToSet });
            
            // Close all UI pages on load
            if (UIOverlayManager.Instance != null)
            {
                UIOverlayManager.Instance.CloseAllPages();
            }

            // Small delay for smooth transition and ensuring frame is rendered
            yield return new WaitForSecondsRealtime(0.5f); 
        }
    }

    private IEnumerator UnloadAllExceptPersistent()
    {
        List<Scene> scenesToUnload = new List<Scene>();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            string name = scene.name;

            bool isPersistent = false;
            if (persistentScenes != null)
            {
                foreach (var pScene in persistentScenes)
                {
                    if (pScene != null && name == pScene.SceneName)
                    {
                        isPersistent = true;
                        break;
                    }
                }
            }

            if (name == "Bootstrap" || name == "PersistentGameplay")
            {
                isPersistent = true;
            }

            if (!isPersistent && scene.isLoaded)
            {
                scenesToUnload.Add(scene);
            }
        }

        List<AsyncOperation> operations = new List<AsyncOperation>();
        foreach (var scene in scenesToUnload)
        {
            try {
                var op = SceneManager.UnloadSceneAsync(scene);
                if (op != null) operations.Add(op);
            } catch (Exception ex) {
                Debug.LogWarning($"Failed to unload scene {scene.name}: {ex.Message}");
            }
        }

        foreach (var op in operations)
        {
            while (!op.isDone) yield return null;
        }
    }

    private IEnumerator LoadScenes(List<SceneField> rawScenesToLoad, SceneField activeSceneToSet, bool showingLoadingBar)
    {
        List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
        HashSet<SceneField> scenesToLoad = FilterScenesToLoad(rawScenesToLoad);

        foreach (SceneField scene in scenesToLoad)
        {
            if (string.IsNullOrEmpty(scene.SceneName)) continue;
            var op = SceneManager.LoadSceneAsync(scene.SceneName, LoadSceneMode.Additive);
            if (op != null)
            {
                scenesLoading.Add(op);
            }
        }

        if (scenesLoading.Count > 0)
        {
            float visualProgress = bar != null ? bar.fillAmount : 0;
            while (true)
            {
                float totalProgress = 0;
                bool allDone = true;
                foreach (var op in scenesLoading)
                {
                    if (op == null) continue;
                    totalProgress += op.progress;
                    if (!op.isDone) allDone = false;
                }

                float targetFill = totalProgress / scenesLoading.Count;
                if (showingLoadingBar && bar != null)
                {
                    // Smoothly interpolate the bar towards the target progress.
                    float displayTarget = targetFill >= 0.9f ? 1.0f : targetFill;
                    visualProgress = Mathf.MoveTowards(visualProgress, displayTarget, Time.unscaledDeltaTime * 2f);
                    bar.fillAmount = visualProgress;
                }

                if (allDone) break;
                yield return null;
            }
        }

        // Finalize bar
        if (showingLoadingBar && bar != null)
        {
            bar.DOKill();
            bool tweenDone = false;
            bar.DOFillAmount(1f, 0.3f)
               .SetUpdate(true)
               .OnComplete(() => tweenDone = true);
            yield return new WaitUntil(() => tweenDone);
        }

        if (!string.IsNullOrEmpty(activeSceneToSet.SceneName))
        {
            Scene s = GetLoadedScene(activeSceneToSet.SceneName);
            int retryCount = 0;
            while ((!s.IsValid() || !s.isLoaded) && retryCount < 100)
            {
                yield return null;
                s = GetLoadedScene(activeSceneToSet.SceneName);
                retryCount++;
            }

            if (s.IsValid() && s.isLoaded)
            {
                SceneManager.SetActiveScene(s);
            }
        }

        yield return new WaitForSecondsRealtime(0.2f);
    }

    private IEnumerator UnloadScenes(List<SceneField> scenesToUnload)
    {
        if (scenesToUnload == null || scenesToUnload.Count == 0) yield break;

        List<AsyncOperation> scenesUnloading = new List<AsyncOperation>();
        foreach (var scene in scenesToUnload)
        {
            if (scene.SceneName == "PersistentGameplay") continue;

            Scene s = GetLoadedScene(scene.SceneName);
            if (s.IsValid() && s.isLoaded)
            {
                var op = SceneManager.UnloadSceneAsync(s);
                if (op != null) scenesUnloading.Add(op);
            }
        }

        foreach (var operation in scenesUnloading)
        {
            while (!operation.isDone) yield return null;
        }
    }

    private HashSet<SceneField> FilterScenesToLoad(List<SceneField> rawScenesToLoad)
    {
        HashSet<SceneField> filteredScenes = new HashSet<SceneField>();
        if (rawScenesToLoad == null) return filteredScenes;

        foreach (var rawScene in rawScenesToLoad)
        {
            if (string.IsNullOrEmpty(rawScene.SceneName)) continue;

            Scene scene = GetLoadedScene(rawScene.SceneName);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                filteredScenes.Add(rawScene);
            }
        }

        return filteredScenes;
    }

    private Scene GetLoadedScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name == sceneName && s.isLoaded)
            {
                return s;
            }
        }
        return default;
    }

    public void ShowLoadingScreen()
    {
        if (loadingCanvasPfb != null && !loadingCanvasPfb.activeSelf)
        {
            loadingCanvasPfb.SetActive(true);
            bar.fillAmount = 0;

            foreach (var cam in Camera.allCameras)
            {
                if (cam.enabled && cam.gameObject != cameraMain)
                {
                    cam.enabled = false;
                    disabledCameras.Add(cam);
                }
            }

            cameraMain.SetActive(true);
            var camComp = cameraMain.GetComponent<Camera>();
            if (camComp != null)
            {
                camComp.enabled = true;
                camComp.depth = 1000;
            }
        }
    }

    public void HideLoadingScreen()
    {
        if (loadingCanvasPfb != null)
        {
            loadingCanvasPfb.SetActive(false);
            cameraMain.SetActive(false);
        }

        foreach (var cam in disabledCameras)
        {
            if (cam != null) cam.enabled = true;
        }
        disabledCameras.Clear();
    }
}
