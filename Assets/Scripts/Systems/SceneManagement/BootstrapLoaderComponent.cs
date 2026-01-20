using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Script that auto loads bootstrap scene if not already loaded.
/// </summary>
public class BootstrapLoaderComponent : MonoBehaviour
{
    [SerializeField] private bool loadOnStart = true;

    private void Start()
    {
        if (loadOnStart) LoadBootstrapScene();
    }

    private void LoadBootstrapScene()
    {
        if (!SceneManager.GetSceneByName("Bootstrap").IsValid())
        {
            // If no current Bootstrap Scene Actively Loaded. Load it.
            SceneManager.LoadScene("Bootstrap", LoadSceneMode.Additive);
        }
    }
}