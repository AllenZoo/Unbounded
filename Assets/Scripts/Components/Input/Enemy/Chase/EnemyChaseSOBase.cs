using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseSOBase : ScriptableObject
{
    // Context
    protected EnemyAIComponent enemyAIComponent;
    public void Initialize(EnemyAIComponent enemyAIComponent)
    {
        this.enemyAIComponent = enemyAIComponent;
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
