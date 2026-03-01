using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStateBase
{
    // Enemy AI Component to access SO from.
    protected EnemyAIComponent enemyAIComponent;

    // Keep track of current state. Also used to change state.
    protected EnemyStateMachine stateMachine;

    public EnemyStateBase(EnemyAIComponent enemyAIComponent, EnemyStateMachine stateMachine)
    {
        this.enemyAIComponent = enemyAIComponent;
        this.stateMachine = stateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }
    public virtual void PhysicsUpdate() { }
    public virtual void AnimationTrigger() { }
}
