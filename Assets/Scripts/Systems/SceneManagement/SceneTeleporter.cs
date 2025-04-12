using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTeleporter : MonoBehaviour
{
    public UnityEvent OnTeleportRequest;

    public void TeleportToScene()
    {
        OnTeleportRequest?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TeleportToScene();
        }
    }
}
