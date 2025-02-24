using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyIdleRandomWalk", menuName = "System/Enemy/State/Idle/RandomWalk")]
public class EnemyIdleSOBase : ScriptableObject
{
    // Context
    protected EnemyAIComponent enemyAIComponent;
    protected GameObject enemyObject;

    public virtual void Initialize(EnemyAIComponent enemyAIComponent, GameObject enemyObject)
    {
        this.enemyAIComponent = enemyAIComponent;
        this.enemyObject = enemyObject;
    }

    public virtual void DoEnterLogic() { }
    public virtual void DoExitLogic() { ResetValues(); }
    public virtual void DoFrameUpdateLogic() { }
    public virtual void DoPhysicsUpdateLogic() { }
    public virtual void DoAnimationTriggerEventLogic() { }
    public virtual void ResetValues() { }
}
