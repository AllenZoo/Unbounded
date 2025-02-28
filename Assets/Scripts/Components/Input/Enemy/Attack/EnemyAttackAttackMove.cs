using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new EnemyAttack Attack Move", menuName = "System/Enemy/State/Attack/AttackMove")]
public class EnemyAttackAttackMove : EnemyAttackSOBase
{

    // Class to emulate both chasing and attack range behaviour.
    [SerializeField] private EnemyChaseSOBase chase;
    [SerializeField] private float attackRange;


    public override void Initialize(EnemyAIComponent enemyAIComponent, GameObject enemyObject, ContextSteerer contextSteerer, ObjectTracker tracker, Transform feetTransform)
    {
        base.Initialize(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);
        chase.Initialize(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);
    }

    public override void DoAnimationTriggerEventLogic()
    {
        base.DoAnimationTriggerEventLogic();
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        // TODO: test taht chase.DoFrameUpdageLogi() will work even though we don't instantiate the SO instance like we do in enemyAiComponent.
        base.DoFrameUpdateLogic();
        chase.DoFrameUpdateLogic();


        Transform targetTransform = enemyAIComponent.AggroTarget.transform;
        Transform thisTransform = enemyObject.transform;

        float dist = Vector2.Distance(thisTransform.position, targetTransform.position);
        // If the target is within attack range, attack
        if (dist < attackRange)
        {
            enemyAIComponent.InvokeAttackInput(KeyCode.K, new AttackSpawnInfo(targetTransform.position));
        }
    }

    public override void DoPhysicsUpdateLogic()
    {
        base.DoPhysicsUpdateLogic();
    }

    

    public override void ResetValues()
    {
        base.ResetValues();
    }
}
