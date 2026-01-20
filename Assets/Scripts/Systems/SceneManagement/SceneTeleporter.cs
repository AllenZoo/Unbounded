using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

[RequiredComponents(typeof(WorldInteractableObject))]
public class SceneTeleporter : SerializedMonoBehaviour
{
    [Tooltip("Conditions that must be true to allow teleporting.")]
    [SerializeField] private ConditionChecker conditionChecker;


    [SerializeField] private bool teleportOnCollision = false;
    [SerializeField] private SceneField targetScene;

    // Can't odin serialize an abstract obejct on pfb, so using plain old hard coded GetComponent init instead.
    private WorldInteractableObject trigger;

    private void Awake()
    {
        trigger.GetComponent<WorldInteractableObject>();

        if (trigger == null)
        {
            Debug.LogError("SceneTeleporter requires a WorldInteractableObject component.");
            return;
        }

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
