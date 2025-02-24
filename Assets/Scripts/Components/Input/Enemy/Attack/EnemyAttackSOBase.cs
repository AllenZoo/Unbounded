using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackSOBase : ScriptableObject
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
    public virtual void DoPhysicsUpdateLogic() { }
    public virtual void DoAnimationTriggerEventLogic() { }
    public virtual void ResetValues() { }
}
