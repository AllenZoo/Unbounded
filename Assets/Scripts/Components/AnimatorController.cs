using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(MotionComponent))]
[RequireComponent(typeof(StateComponent))]
[RequireComponent(typeof(Animator))]
public class AnimatorController : MonoBehaviour
{
    // Animation Names
    public static string IDLE_ANIMATION_NAME = "Idle";
    public static string WALKING_ANIMATION_NAME = "Walking";
    public static string RUNNING_ANIMATION_NAME = "Running";

    // Animator Parameter Names
    public static string DIRECTION_PARAMETER_X = "xDir";
    public static string DIRECTION_PARAMETER_Y = "yDir";
    public static string LAST_DIRECTION_PARAMETER_X = "last_xDir";
    public static string LAST_DIRECTION_PARAMETER_Y = "last_yDir";

    // Update this whenever new parameter is added.
    public static float NUMBER_OF_PARAMETERS = 4;

    private MotionComponent motion;
    private StateComponent state;
    private Animator animator;

    private void Awake()
    {
        motion = GetComponent<MotionComponent>();
        state = GetComponent<StateComponent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Debug.Assert(motion != null, "Motion null in animation controller for object: " + gameObject);
        Debug.Assert(state != null, "State null in animation controller for object: " + gameObject);
        Debug.Assert(animator != null, "Animator null in animation controller for object: " + gameObject);
        state.OnStateChanged += State_OnStateChanged;
        motion.OnMotionChange += Motion_OnMotionChange;


        // Check Parameters are present in animator controller
        Debug.Assert(animator.parameters.Length >= NUMBER_OF_PARAMETERS, 
            "Animator parameters not set up correctly for object: " + gameObject);

        // Init Parameters in animator (TODO: once it gets large, move to helper)
        animator.SetFloat(DIRECTION_PARAMETER_X, 0);
        animator.SetFloat(DIRECTION_PARAMETER_Y, 0);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_X, 0);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_Y, 0);
    }

    private void Motion_OnMotionChange(Vector2 obj)
    {
        SetMovementParameters();
    }

    private void State_OnStateChanged(State oldState, State newState)
    {
        Handle_Animation();
    }

    private void SetMovementParameters()
    {
        animator.SetFloat(DIRECTION_PARAMETER_X, motion.dir.x);
        animator.SetFloat(DIRECTION_PARAMETER_Y, motion.dir.y);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_X, motion.lastDir.x);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_Y, motion.lastDir.y);
    }

    // Modifies the state of the animator 
    private void Handle_Animation()
    {
        switch (state.state)
        {
            case State.IDLE:
                animator.Play(IDLE_ANIMATION_NAME);
                break;
            case State.WALKING:
                animator.Play(WALKING_ANIMATION_NAME);
                break;
            case State.RUNNING:
                animator.Play(RUNNING_ANIMATION_NAME);
                break;
            default:
                Debug.LogWarning("Implement animator for state: " + state.state.ToString());
                //Debug.LogError("Encountered invalid state: " + state.state.ToString());
                break;
        }
    }

}
