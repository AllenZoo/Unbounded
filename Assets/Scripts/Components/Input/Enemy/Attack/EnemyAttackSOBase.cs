using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackSOBase : SerializedScriptableObject
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
    public virtual void DoFrameUpdateLogic() {
        if (enemyAIComponent.AggroTarget == null)
        {
            // Back to idle if no more aggro
            enemyAIComponent.StateMachine.ChangeState(enemyAIComponent.EnemyIdleState);
            return;
        };

        float dist = Vector2.Distance(feetTransform.position, enemyAIComponent.AggroTarget.transform.position);
        if (dist > enemyAIComponent.AttackRange)
        {
            enemyAIComponent.StateMachine.ChangeState(enemyAIComponent.EnemyChaseState);
        }
    }
    public virtual void DoPhysicsUpdateLogic() { }
    public virtual void DoAnimationTriggerEventLogic() { }
    public virtual void ResetValues() { }
}
