using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


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

    /// <summary>
    /// Reference to disabled cameras so that our main camera is the one rendering the loading screen.
    /// Useful for re-enabling after loading is done.
    /// </summary>
    private List<Camera> disabledCameras = new List<Camera>();


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
        EventBus<OnSceneLoadRequestFinish>.Call(new OnSceneLoadRequestFinish() { finishedActiveScene = e.activeSceneToSet});
        yield return new WaitForSecondsRealtime(1f); // Delay a bit so that scene transition more smooth. (if we dont do this, we will see previous frame for a split second before transition). This due to how our pause system affects camera.
        EventBus<OnPauseChangeRequest>.Call(new OnPauseChangeRequest() { shouldPause = false });
        HideLoadingScreen();

        yield return null;
    }
    private IEnumerator LoadScenes(List<SceneField> rawScenesToLoad, SceneField activeSceneToSet, bool showingLoadingBar)
    {
        if (!string.IsNullOrEmpty(activeSceneToSet.SceneName) &&
            !rawScenesToLoad.Contains(activeSceneToSet))
        {
            Debug.LogError(
                $"Active scene '{activeSceneToSet}' is not included in scenesToLoad!"
            );
        }


        List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
        HashSet<SceneField> scenesToLoad = FilterScenesToLoad(rawScenesToLoad);
        Scene loadedActiveScene = default;

        foreach (SceneField scene in scenesToLoad)
        {
            var op = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            scenesLoading.Add(op);

            if (scene == activeSceneToSet)
            {
                op.completed += _ =>
                {
                    loadedActiveScene = SceneManager.GetSceneByName(scene.SceneName);
                };
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

                if (showingLoadingBar) bar.DOFillAmount(targetFill, 0.5f).SetUpdate(true);

                //Debug.Log($"Percentage Loaded {totalProgress / scenesLoading.Count * 100} %");
                yield return null; // NOTE: IMPORTANT DO NOT REMOVE THIS LINE OR IT WILL INFINITE LOOP.
            }
        }

        if (showingLoadingBar) {
            bar.DOFillAmount(1f, 0.6f)
               .SetUpdate(true) // uses unscaled time
               .OnComplete(() => {
                   tweenComplete = true;
#if UNITY_EDITOR
                   Debug.Log("Tween completed");
#endif
               });
        }


        if (!string.IsNullOrEmpty(activeSceneToSet))
        {
            yield return new WaitUntil(() =>
            {
                Scene s = SceneManager.GetSceneByName(activeSceneToSet.SceneName);
                return s.isLoaded;
            });

            // Set active scene if there is one.
            Scene activeScene = SceneManager.GetSceneByName(activeSceneToSet.SceneName);
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
            if (scene.SceneName == "PersistentGameplay")
            {
                Debug.LogError("BIG NONO: Tried to disable persistent gameplay! Make sure you have the correct scene set as active");
                continue;
            }

            // Check if there is a scene to unload.
            Scene s = SceneManager.GetSceneByName(scene);

            if (s.isLoaded)
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

            Scene scene = SceneManager.GetSceneByName(rawScene);

            // Only load if it is NOT loaded
            if (!scene.isLoaded)
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
        if (loadingCanvasPfb != null) { 
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