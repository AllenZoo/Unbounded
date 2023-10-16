using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateComponent : MonoBehaviour
{
    public event Action<State> OnStateChanged;
    public State state { get; private set; } = State.IDLE;

    public void SetState(State state)
    {
        this.state = state;
        OnStateChanged?.Invoke(this.state);
    }
}
