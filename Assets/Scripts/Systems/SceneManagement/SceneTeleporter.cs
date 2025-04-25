using UnityEngine;
using UnityEngine.Events;

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
