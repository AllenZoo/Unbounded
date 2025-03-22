using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseSOBase : SerializedScriptableObject
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
    /// <summary>
    /// Frame update logic for enemy states. If stateChange = true, we will change states if conditions satisfied. Otherwise, we don't.
    /// Disabling stateChange is useful if we want to reuse some EnemyChaseSOBase logic but aren't actually in the state.
    /// </summary>
    /// <param name="stateChange"></param>
    public virtual void DoFrameUpdateLogic(bool stateChange=true) {
        if (enemyAIComponent.AggroTarget == null) return;

        float dist = Vector2.Distance(feetTransform.position, enemyAIComponent.AggroTarget.transform.position);
        if (stateChange && dist < enemyAIComponent.AttackRange)
        {
            enemyAIComponent.StateMachine.ChangeState(enemyAIComponent.EnemyAttackState);
        }
    }
    public virtual void DoPhysicsUpdateLogic() { }
    public virtual void DoAnimationTriggerEventLogic() { }
    public virtual void ResetValues() { }

    
}
