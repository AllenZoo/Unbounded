using NUnit.Framework;
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
            if (!SceneManager.GetSceneByName(scene.SceneName).IsValid())
            {
                // If no current Scene Actively Loaded. Load it.
                SceneManager.LoadScene(scene.SceneName, LoadSceneMode.Additive);
            }
        }
    }
}