using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

/// <summary>
/// Wrapper for Animator.
/// 
/// Takes in AnimationState and does stuff based on that input.
/// 
/// Controls a bunch of graphics stuff.
/// </summary>
public class AnimatorController : MonoBehaviour
{
    // Components
    [SerializeField] private Animator animator;

    #region Animator Parameters
    // Animation Names
    public static string IDLE_ANIMATION_NAME = "Idle";
    public static string WALKING_ANIMATION_NAME = "Walking";
    public static string RUNNING_ANIMATION_NAME = "Running";
    public static string ATTACKING_ANIMATION_NAME = "Attacking";
    public static string STUNNED_ANIMATION_NAME = "Stunned";
    public static string DAMAGED_ANIMATION_NAME = "Damaged";

    // Update this whenever a new animation is added.
    // TODO: incorporate this in checking for animation clips.
    
    // Cached Hashes for performance and to avoid missing state warnings
    private static readonly int IdleHash = Animator.StringToHash(IDLE_ANIMATION_NAME);
    private static readonly int WalkingHash = Animator.StringToHash(WALKING_ANIMATION_NAME);
    private static readonly int RunningHash = Animator.StringToHash(RUNNING_ANIMATION_NAME);
    private static readonly int AttackingHash = Animator.StringToHash(ATTACKING_ANIMATION_NAME);
    private static readonly int StunnedHash = Animator.StringToHash(STUNNED_ANIMATION_NAME);
    private static readonly int DamagedHash = Animator.StringToHash(DAMAGED_ANIMATION_NAME);

    private static int ANIMATION_CLIP_COUNT = 6;

    // Animator Parameter Names
    public static string DIRECTION_PARAMETER_X = "xDir";
    public static string DIRECTION_PARAMETER_Y = "yDir";
    public static string LAST_DIRECTION_PARAMETER_X = "last_xDir";
    public static string LAST_DIRECTION_PARAMETER_Y = "last_yDir";

    // Update this whenever new parameter is added.
    public static float NUMBER_OF_PARAMETERS = 4;

    #endregion


    private void Awake()
    {
        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator, "Animator null in animation controller for object: " + gameObject);
    }

    private void Start()
    {
        // Init Parameters in animator (TODO: once it gets large, move to helper)
        animator.SetFloat(DIRECTION_PARAMETER_X, 0);
        animator.SetFloat(DIRECTION_PARAMETER_Y, 0);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_X, 0);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_Y, 0);
    }

    /// <summary>
    /// Main entrypoint function that passes in state data for AnimatorController to handle.
    /// </summary>
    /// <param name="state"></param>
    public void UpdateAnimator(AnimatorState state)
    {
        SetAnimatorDirectionParameters(state.Direction, state.LastFullDirection);
        HandleAnimation(state.State);
    }

    private void SetAnimatorDirectionParameters(Vector2 dir, Vector2 lastDir)
    {
        animator.SetFloat(DIRECTION_PARAMETER_X, dir.x);
        animator.SetFloat(DIRECTION_PARAMETER_Y, dir.y);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_X, lastDir.x);
        animator.SetFloat(LAST_DIRECTION_PARAMETER_Y, lastDir.y);
    }

    private State? currentState;

    // Modifies the state of the animator 
    private void HandleAnimation(State state)
    {
        if (animator == null || !animator.isActiveAndEnabled || animator.runtimeAnimatorController == null) return;
        if (animator.layerCount <= 0) return;

        if (state == currentState) return;
        currentState = state;

        switch (state)
        {
            case State.IDLE:
                if (animator.HasState(0, IdleHash)) animator.Play(IdleHash);
                break;
            case State.WALKING:
                if (animator.HasState(0, WalkingHash)) animator.Play(WalkingHash);
                break;
            case State.RUNNING:
                if (animator.HasState(0, RunningHash)) animator.Play(RunningHash);
                break;
            case State.STUNNED:
                if (animator.HasState(0, StunnedHash)) animator.Play(StunnedHash);
                break;
            case State.ATTACKING:
                if (animator.HasState(0, AttackingHash)) animator.Play(AttackingHash);
                break;
            case State.DAMAGED:
                if (animator.HasState(0, DamagedHash)) animator.Play(DamagedHash);
                break;
            case State.DEAD:
                break;
            default:
                Debug.LogWarning("Implement animator for state: " + state.ToString());
                break;
        }
    }

}
