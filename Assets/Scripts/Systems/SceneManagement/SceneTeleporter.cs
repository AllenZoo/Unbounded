using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class SceneTeleporter : SerializedMonoBehaviour
{
    [Tooltip("Conditions that must be true to allow teleporting.")]
    [SerializeField] private ConditionChecker conditionChecker;


    [SerializeField] private bool teleportOnCollision = false;
    [Required, SerializeField] private SceneField targetScene;

    // Can't odin serialize an abstract obejct on pfb, so using plain old hard coded GetComponent init instead.
    // Note: Since MenuButton also uses this script, we allow for it to be null.
    private WorldInteractableObject trigger;

    private void Awake()
    {
        TryGetComponent<WorldInteractableObject>(out var t);
        trigger = t;
    }

    private void Start()
    {
        if (trigger == null)
        {
            Debug.LogWarning("SceneTeleporter requires a WorldInteractableObject component.");
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

    public void SetTargetScene(SceneField scene)
    {
        if (scene != null)
        {
            targetScene = scene; 
        }
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
