using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour
{
    [SerializeField] private string sceneToTeleportTo;

    public void TeleportToScene()
    {
        if (!string.IsNullOrEmpty(sceneToTeleportTo))
        {
            SceneManager.LoadScene(sceneToTeleportTo);
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
