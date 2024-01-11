using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keeps track of the state of the gameobject and uses that info to perform 
// some logic.
public class StateComponent : MonoBehaviour
{
    // Old State, and New State.
    public event Action<State, State> OnStateChanged;
    public State state { get; private set; } = State.IDLE;

    // Can be null.
    [Header("Can be null. Set if we want to control behaviours of animation, movement, and input based on state.")]
    [SerializeField] private AnimatorController animatorController;
    [SerializeField] private MovementController movementController;
    [SerializeField] private InputController inputController;

    [Header("Can be null. Set if we have components (Damageable/Knockbackable) that set state.")]
    [SerializeField] private Damageable damageable;
    [SerializeField] private Knockbackable knockbackable;


    [Header("For debugging, doesn't affect anything.")]
    [SerializeField] State debuggingState = State.IDLE;
    [SerializeField] private List<State> crowdControlStates = new List<State>() {State.STUNNED };

    private delegate IEnumerator AnimationCoroutine();
    private delegate void StateAction();

    private void Awake()
    {
        if (animatorController == null || movementController == null || inputController == null) {
            Debug.LogWarning("Have not serialized animatorController or movementController or inputController." +
                " This means that animations, movement, and input behaviour" +
                "will not be affected by States for object: " + gameObject.name);
        }
    }

    private void Start()
    {
        OnStateChanged += HandleStateChanged;

        if (knockbackable != null)
        {
            knockbackable.OnKnockBackBegin += (Vector2 dir, float force) =>
            {
                ReqStateChange(State.STUNNED);
            };

            knockbackable.OnKnockBackEnd += () =>
            {
                ReqStateChange(State.CCFREE);
            };
        }

        if (damageable != null)
        {
            damageable.OnDeath += () =>
            {
                ReqStateChange(State.DEAD);
            };

            // TODO: could change state based on how much damage is taken. eg. more dmg = more damaged state.
            //       for future enhancements.
            damageable.OnDamage += (float dmg) =>
            {
                // TODO: instead of changing state, just invoke an 'Damaged' effect in AnimatorController from here.
                // ReqStateChange(State.DAMAGED);
                if (state != State.DEAD)
                {
                    if (animatorController != null)
                    {
                        animatorController.PlayDamagedEffect(dmg);
                    }
                }
                
            };
        }
    }

    // If current state is not part of a CC state (crowd controlled state), then change state.
    // Otherwise, a CC state can only be changed into CCFREE state, or into a higher priority CC state.
    // If dead, cannot switch to any other state. (unless maybe new State 'Revived' we add)
    public void ReqStateChange(State newState)
    {
        if (newState == state)
        {
            return;
        }

        if (state == State.DEAD)
        {
            return;
        }

        if (crowdControlStates.Contains(state))
        {
            if (newState == State.CCFREE)
            {
                SetState(newState);
            }
        }
        else
        {
            SetState(newState);
        }
    }

    public List<State> GetCCStates
    {
        get { return crowdControlStates; }
    }

    private void SetState(State state)
    {
        State oldState = this.state;
        this.state = state;
        OnStateChanged?.Invoke(oldState, this.state);

        // Debugging
        debuggingState = state;
    }

    private void HandleStateChanged(State oldState, State newState)
    {
        // TODO: handle destroying enemy somewhere else. Maybe even make a pool!
        if (newState == State.DEAD)
        {
            // Destroy(gameObject, 1.0f);
            StartCoroutine(WaitThenCall(DeactivateEntity, 1.0f));
        }

        if (animatorController != null)
        {
            animatorController.SetState(newState);
            switch (newState)
            {
                case State.DEAD:
                    animatorController.CanTransitionState = false;
                    break;
                default:
                    animatorController.CanTransitionState = true;
                    break;
            }
        }
        if (movementController != null)
        {
            switch (newState)
            {
                case State.STUNNED:
                    movementController.SetMovementEnabled(false);
                    break;
                case State.DEAD:
                    movementController.SetMovementEnabled(false);
                    movementController.ResetMovementVelocity();
                    break;
                default:
                    movementController.SetMovementEnabled(true);
                    break;
            }
        }

        if (inputController != null)
        {
            switch (newState)
            {
                // fall through
                case State.STUNNED:
                case State.DEAD:
                    inputController.inputEnabled = false;
                    break;
                default:
                    inputController.inputEnabled = true;
                    break;
            }
        }
    }

    private IEnumerator WaitForClipEnd(AnimationCoroutine coroutine, StateAction onEnd)
    {
        yield return StartCoroutine(coroutine());
        onEnd();
        // ReqStateChange(State.CCFREE);
    }
    private IEnumerator WaitThenCall (StateAction onEnd, float time)
    {
        yield return new WaitForSeconds(time);
        onEnd();
    }

    private void DeactivateEntity()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }
}
