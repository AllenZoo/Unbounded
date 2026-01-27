using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;

public class SceneTeleporter : SerializedMonoBehaviour
{
    [Tooltip("Conditions that must be true to allow teleporting.")]
    [SerializeField] private ConditionChecker conditionChecker;

    [Required, SerializeField] private SceneField targetScene;
    [Optional, SerializeField] private ModalContext modalContext;
    [Optional, SerializeField] private ModalData teleportModalData;

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

        trigger.OnInteract.AddListener(OnInteract);
    }

    public void SetTargetScene(SceneField scene)
    {
        if (scene != null)
        {
            targetScene = scene;
        }
    }

    /// <summary>
    /// Triggers intent to teleport to the target scene.
    /// 
    /// Opens modal if specified.
    /// </summary>
    private void OnInteract()
    {
        var intent = new TeleportInteractionIntent(targetScene);
        conditionChecker.ValidateConditions();

        if (modalContext != null && teleportModalData != null)
        {
            modalContext.Open(teleportModalData, intent);
        }
        else
        {
            intent.Commit();
        }

    }
}
