using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseSOBase : ScriptableObject
{
    // Context
    protected EnemyAIComponent enemyAIComponent;
    protected GameObject enemyObject;
    protected ContextSteerer contextSteerer;
    protected ObjectTracker tracker;
    protected Transform feetTransform;
    public virtual void Initialize(EnemyAIComponent enemyAIComponent, GameObject enemyObject, ContextSteerer contextSteerer, ObjectTracker tracker, Transform feetTransform)
    {
        this.enemyAIComponent = enemyAIComponent;
        this.enemyObject = enemyObject;
        this.contextSteerer = contextSteerer;
        this.tracker = tracker;
        this.feetTransform = feetTransform;
    }

    public virtual void DoEnterLogic() { }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic() { }
    public virtual void DoPhysicsUpdateLogic() {
        // TODO: remove. Here is just sample state changing code!
        bool cond = false;
        if (cond)
        {
            enemyAIComponent.StateMachine.ChangeState(enemyAIComponent.EnemyAttackState);
        }

    }
    public virtual void DoAnimationTriggerEventLogic() { }
    public virtual void ResetValues() { }

    
}
