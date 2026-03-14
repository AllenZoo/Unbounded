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

    /// <summary>
    /// Reference to disabled cameras so that our main camera is the one rendering the loading screen.
    /// Useful for re-enabling after loading is done.
    /// </summary>
    private List<Camera> disabledCameras = new List<Camera>();

    private bool isLoading = false;


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
        if (isLoading)
        {
            Debug.LogWarning("Scene loading already in progress. Ignoring request.");
            return;
        }
        StartCoroutine(OnSceneLoadRequestEventCoroutine(e));
    }

    private IEnumerator OnSceneLoadRequestEventCoroutine(OnSceneLoadRequest e)
    {
        isLoading = true;
        try
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
                yield return StartCoroutine(LoadScenes(e.scenesToLoad, e.activeSceneToSet, e.showLoadingBar));
                EventBus<OnSceneLoadRequestFinish>.Call(new OnSceneLoadRequestFinish() { finishedActiveScene = e.activeSceneToSet });
                yield return new WaitForSecondsRealtime(1f); // Delay a bit so that scene transition more smooth. (if we dont do this, we will see previous frame for a split second before transition). This due to how our pause system affects camera.
            }
        }
        finally
        {
            HideLoadingScreen();
            isLoading = false;
        }
        yield return null;
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

            // Always keep Bootstrap and PersistentGameplay
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
            var op = SceneManager.UnloadSceneAsync(scene);
            if (op != null) operations.Add(op);
        }

        foreach (var op in operations)
        {
            yield return new WaitUntil(() => op.isDone);
        }
    }

    private IEnumerator LoadScenes(List<SceneField> rawScenesToLoad, SceneField activeSceneToSet, bool showingLoadingBar)
    {
        if (!string.IsNullOrEmpty(activeSceneToSet.SceneName) &&
            !rawScenesToLoad.Contains(activeSceneToSet))
        {
            Debug.LogError(
                $"Active scene '{activeSceneToSet.SceneName}' is not included in scenesToLoad!"
            );
        }


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
            else
            {
                Debug.LogError($"Failed to load scene '{scene.SceneName}'. It might not be in the Build Settings.");
            }
        }


        bool tweenComplete = false;
        if (scenesLoading.Count > 0)
        {
            float visualProgress = 0;
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
                    // This creates a much more fluid motion than snapping to raw progress values.
                    visualProgress = Mathf.Lerp(visualProgress, targetFill, Time.unscaledDeltaTime * 5f);
                    
                    // Ensure it never goes backwards and actually reaches the target if it's close.
                    if (targetFill - visualProgress < 0.01f) visualProgress = targetFill;
                    
                    bar.fillAmount = visualProgress;
                }

                if (allDone) break;
                yield return null;
            }
        }

        if (showingLoadingBar && bar != null)
        {
            bar.DOKill();
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
        else
        {
            tweenComplete = true;
        }


        if (!string.IsNullOrEmpty(activeSceneToSet.SceneName))
        {
            yield return new WaitUntil(() =>
            {
                Scene s = GetLoadedScene(activeSceneToSet.SceneName);
                return s.IsValid() && s.isLoaded;
            });

            // Set active scene if there is one.
            Scene activeScene = GetLoadedScene(activeSceneToSet.SceneName);
            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }
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
            if (scene.SceneName == "PersistentGameplay")
            {
                Debug.LogError("BIG NONO: Tried to disable persistent gameplay! Make sure you have the correct scene set as active");
                continue;
            }

            // Check if there is a scene to unload.
            Scene s = GetLoadedScene(scene.SceneName);

            if (s.IsValid() && s.isLoaded)
            {
                var op = SceneManager.UnloadSceneAsync(s);
                if (op != null) scenesUnloading.Add(op);
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
            if (string.IsNullOrEmpty(rawScene.SceneName)) continue;

            // Only load if it is NOT already loaded
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
        if (loadingCanvasPfb != null)
        {
            loadingCanvasPfb.SetActive(true);
            bar.fillAmount = 0;

            // ensure it's rendered
            var canvas = loadingCanvasPfb.GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            // disable other cams
            foreach (var cam in Camera.allCameras)
            {
                if (cam.enabled)
                {
                    cam.enabled = false;
                    disabledCameras.Add(cam);
                }
            }

            // activate main camera
            cameraMain.SetActive(true);
            var camComp = cameraMain.GetComponent<Camera>();
            if (camComp != null)
            {
                camComp.enabled = true;
                camComp.depth = 1000; // ensure on top
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

        // Re-enable previously disabled cameras
        foreach (var cam in disabledCameras)
        {
            if (cam != null)
            {
                cam.enabled = true;
            }
        }
        disabledCameras.Clear();
    }
}
