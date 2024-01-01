using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateComponent : MonoBehaviour
{
    // Old State, and New State.
    public event Action<State, State> OnStateChanged;
    public State state { get; private set; } = State.IDLE;

    [Header("For debugging, doesn't affect anything.")]
    [SerializeField] State debuggingState = State.IDLE;
    public void SetState(State state)
    {
        State oldState = this.state;
        this.state = state;
        OnStateChanged?.Invoke(oldState, this.state);

        // Debugging
        debuggingState = state;
    }
}
