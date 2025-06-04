using UnityEngine;
using UnityEngine.Events;

public class SceneTeleporter : MonoBehaviour
{
    [Tooltip("Conditions that must be true to allow teleporting.")]
    [SerializeField] private ConditionChecker conditionChecker;

    [Tooltip("Event called when teleport conditions are met.")]
    public UnityEvent OnTeleportRequest;

    public void TeleportToScene()
    {
        OnTeleportRequest?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // Check conditions before teleporting
        if (conditionChecker == null || conditionChecker.ValidateConditions())
        {
            TeleportToScene();
        }
        else
        {
            Debug.Log("Teleport blocked: Conditions not met.");
        }
    }
}
