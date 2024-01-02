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
    [Header("Can be null. Set if we want to control behaviours based on state.")]
    [SerializeField] private AnimatorController animatorController;
    [SerializeField] private MovementController movementController;

    [SerializeField] private Damageable damageable;
    [SerializeField] private Knockbackable knockbackable;

    [Header("For debugging, doesn't affect anything.")]
    [SerializeField] State debuggingState = State.IDLE;
    [SerializeField] private List<State> crowdControlStates = new List<State>() {State.STUNNED };

    private void Awake()
    {
        if (animatorController == null || movementController == null) {
            Debug.LogWarning("Have not serialized animatorController or movementController. This means that animations and movement requests" +
                "will not be affected by States");
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
    }

    // If current state is not part of a CC state (crowd controlled state), then change state.
    // Otherwise, a CC state can only be changed into CCFREE state, or into a higher priority CC state.
    public void ReqStateChange(State newState)
    {
        if (newState == state)
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
        if (animatorController != null)
        {
            // animatorController.SetState(newState);
        }
        if (movementController != null)
        {
            switch (newState)
            {
                case State.STUNNED:
                    movementController.SetMovementEnabled(false);
                    break;
                default:
                    movementController.SetMovementEnabled(true);
                    break;
            }
        }
    }
}
