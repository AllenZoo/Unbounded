using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class SceneTeleporter : SerializedMonoBehaviour
{
    [OdinSerialize, Required] private WorldInteractableObject trigger;

    [Tooltip("Conditions that must be true to allow teleporting.")]
    [SerializeField] private ConditionChecker conditionChecker;

    // Note:
    //[Tooltip("Event called when teleport conditions are met.")]
    //public UnityEvent OnTeleportRequest;

    [SerializeField] private bool teleportOnCollision = false;
    [SerializeField] private SceneField targetScene;

    private void Awake()
    {
        trigger.OnInteract.AddListener(TeleportToScene);
    }

    public void TeleportToScene()
    {
        EventBus<OnSceneTeleportRequest>.Call(new OnSceneTeleportRequest
        {
            targetScene = targetScene
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // Check conditions before teleporting
        if (conditionChecker == null || conditionChecker.ValidateConditions())
        {
            if (teleportOnCollision)
            {
                TeleportToScene();
            }
        }
        else
        {
            Debug.Log("Teleport blocked: Conditions not met.");
        }
    }
}
