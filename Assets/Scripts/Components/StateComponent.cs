using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateComponent : MonoBehaviour
{
    // Old State, and New State.
    public event Action<State, State> OnStateChanged;
    public State state { get; private set; } = State.IDLE;

    public void SetState(State state)
    {
        State oldState = this.state;
        this.state = state;
        OnStateChanged?.Invoke(oldState, this.state);
    }
}
