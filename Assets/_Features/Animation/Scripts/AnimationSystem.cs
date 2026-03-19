using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The GLUE class.
/// Builds Animator State Struct using Builder, then passes it through to Animator Controller to use.
/// </summary>
public class AnimationSystem : MonoBehaviour
{
    // Glue via LEH
    [Required, SerializeField] private LocalEventHandler leh;

    // External Systems we need to glue.
    [Required, SerializeField] private MotionComponent motionComponent;
    [Required, SerializeField] private StateComponent stateComponent;

    // Animation System components.
    [Required, SerializeField] private AnimatorController controller;
    private AnimatorStateBuilder builder;

    private void Awake()
    {
        leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);

        Assert.IsNotNull(leh, "LocalEventHandler must be assigned in inspector for AnimationSystem to work.");
        Assert.IsNotNull(motionComponent, "MotionComponent must be assigned in inspector for AnimationSystem to work.");
        Assert.IsNotNull(stateComponent, "StateComponent must be assigned in inspector for AnimationSystem to work.");
        Assert.IsNotNull(controller, "AnimatorController must be assigned in inspector for AnimationSystem to work.");

        builder = new AnimatorStateBuilder.Builder().Build(motionComponent, stateComponent);
    }

    private void OnEnable()
    {
        motionComponent.OnMotionChanged.Subscribe(UpdateAnimatorState);
        stateComponent.OnStateChanged.Subscribe(UpdateAnimatorState);
    }

    private void OnDisable()
    {
        motionComponent.OnMotionChanged.Unsubscribe(UpdateAnimatorState);
        stateComponent.OnStateChanged.Unsubscribe(UpdateAnimatorState);
    }

    /// <summary>
    /// Main function that connects the builder to the controller.
    /// It gets called whenever any relevant data changes in dependent systems.
    /// 
    /// </summary>
    private void UpdateAnimatorState()
    {
        AnimatorState newStruct = builder.CreateAnimatorState();
        controller.UpdateAnimator(newStruct);
    }


}
