using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneTeleporter : SerializedMonoBehaviour
{
    [Tooltip("Conditions that must be true to allow teleporting.")]
    [SerializeField] private ConditionChecker conditionChecker;

    [Required, SerializeField] private SceneField targetScene;

    [Tooltip("If true, all other currently loaded scenes will be unloaded when teleporting. (apart from persistent scenes)")]
    [Required, SerializeField] private bool unloadAllOtherScenesButPersist = true;

    [Optional, SerializeField] private ScriptableObjectBoolean varToSetTrueOnYesAnswerModal; // Super ugly but works for now :)

    [Tooltip("Modal will display if this condition is TRUE.")]
    [Optional, SerializeField] private BooleanPreconditionData modalDisplayCondition;
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
    /// Entrypoint for invoking via inspector.
    /// </summary>
    public void Teleport()
    {
        OnInteract();
    }

    /// <summary>
    /// Triggers intent to teleport to the target scene.
    /// 
    /// Opens modal if specified.
    /// </summary>
    private void OnInteract()
    {
        //if (trigger != null && !trigger.CanInteract(out _))
        //    return;

        // TODO: check when this function is called from flow.
        var teleportIntent = new TeleportInteractionIntent(targetScene, unloadAllOtherScenesButPersist);
        var setBoolIntent = new SetBooleanVariableIntent(varToSetTrueOnYesAnswerModal, true);

        conditionChecker?.ValidateConditions();

        if (modalContext != null && teleportModalData != null)
        {
            if (modalDisplayCondition != null && !modalDisplayCondition.IsMet())
            {
                teleportIntent.Commit();
                //setBoolIntent.Commit();
                return;
            }

            var intentList = new List<ICommittableInteraction>() { teleportIntent, setBoolIntent };
            modalContext.Open(teleportModalData, intentList);
        }
        else
        {
            teleportIntent.Commit();
            //setBoolIntent.Commit();
        }
    }
}
