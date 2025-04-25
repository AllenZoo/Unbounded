public class EnemyStateMachine
{
    public EnemyStateBase CurrentEnemyState { get; set; }

    /// <summary>
    /// Initializes the State Machine to given state
    /// </summary>
    /// <param name="initState"></param>
    public void Initialize(EnemyStateBase initState)
    {
        CurrentEnemyState = initState;
        initState.EnterState();
    }

    /// <summary>
    /// Exits old statem, enterse new given state.
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(EnemyStateBase newState)
    {
        CurrentEnemyState.ExitState();
        CurrentEnemyState = newState;
        CurrentEnemyState.EnterState();
    }

}
