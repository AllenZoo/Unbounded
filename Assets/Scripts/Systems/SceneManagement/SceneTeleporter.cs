using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour
{
    [SerializeField] private SceneField sceneToTeleportTo;

    public void TeleportToScene()
    {
        if (!string.IsNullOrEmpty(sceneToTeleportTo))
        {
            // TODO: fix up how we load and unload scenes (OUT OF SCOPE FOR CURRENT ISSUE)
            SceneManager.LoadScene(sceneToTeleportTo, LoadSceneMode.Additive);
        }
        else
        {
            Debug.LogError("Scene to teleport to is not set!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TeleportToScene();
        }
    }
}
