using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SceneTeleporter : WorldInteractableObject
{
    [SerializeField, Required] private InteractablePromptData sceneTeleporterPromptData;

    [Tooltip("Conditions that must be true to allow teleporting.")]
    [SerializeField] private ConditionChecker conditionChecker;

    [Tooltip("Event called when teleport conditions are met.")]
    public UnityEvent OnTeleportRequest;

    [SerializeField] private bool teleportOnCollision = false;

    private void Awake()
    {
        InteractablePromptData newPrompt = sceneTeleporterPromptData;
        messageDisplayBehaviour = new MessageDisplay(soPromptData, newPrompt);
    }

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

    public override void Interact()
    {
        OnTeleportRequest?.Invoke();
    }

    public override void UnInteract()
    {

    }
}
