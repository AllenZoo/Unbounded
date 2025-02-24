using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    private EnemyStateBase currentState;
    
    /// <summary>
    /// Initializes the State Machine to given state
    /// </summary>
    /// <param name="initState"></param>
    public void Initialize(EnemyStateBase initState)
    {
        currentState = initState;
        initState.EnterState();
    }

    /// <summary>
    /// Exits old statem, enterse new given state.
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(EnemyStateBase newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }
}
