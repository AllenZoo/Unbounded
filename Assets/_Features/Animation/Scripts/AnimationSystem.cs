using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// The GLUE class.
/// Builds Animator State Struct using Builder, then passes it through to Animator Controller to use.
/// </summary>
public class AnimationSystem : MonoBehaviour
{
    // External Systems we need to glue.
    [Required, SerializeField] private MotionComponent motionComponent;
    [Required, SerializeField] private StateComponent stateComponent;

    // Animation System components.
    [Required, SerializeField] private AnimatorController controller;
    private AnimatorStateBuilder builder;

    private void Awake()
    {
        Assert.IsNotNull(motionComponent, "MotionComponent must be assigned in inspector for AnimationSystem to work.");
        Assert.IsNotNull(stateComponent, "StateComponent must be assigned in inspector for AnimationSystem to work.");
        Assert.IsNotNull(controller, "AnimatorController must be assigned in inspector for AnimationSystem to work.");

        builder = new AnimatorStateBuilder.Builder().Build(motionComponent, stateComponent);
    }

    private void OnEnable()
    {
        motionComponent.OnMotionChanged.Subscribe(UpdateAnimatorState);
    }

    private void OnDisable()
    {
        motionComponent.OnMotionChanged.Unsubscribe(UpdateAnimatorState);
    }

    /// <summary>
    /// Main function that connects the builder to the controller.
    /// It gets called whenever any relevant data changes in dependent systems.
    /// 
    /// </summary>
    private void UpdateAnimatorState()
    {
        AnimatorState newStruct = builder.CreateAnimatorState();
        //controller.SetAnimationState(newStruct);
    }


}
