
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that auto loads specified scene if not already loaded.
/// </summary>
public class SceneLoaderComponent : MonoBehaviour
{
    [SerializeField] private List<SceneField> scenesToLoad;
    [SerializeField] private bool loadOnStart = true;

    private void Start()
    {
        if (loadOnStart) LoadScenes();
    }

    private void LoadScenes()
    {
        foreach (SceneField scene in scenesToLoad)
        {
            if (string.IsNullOrEmpty(scene.SceneName)) continue;

            // Robust check to see if scene is already LOADED
            if (!IsSceneLoaded(scene.SceneName))
            {
                SceneManager.LoadScene(scene.SceneName, LoadSceneMode.Additive);
            }
        }
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name == sceneName && s.isLoaded)
            {
                return true;
            }
        }
        return false;
    }
}
