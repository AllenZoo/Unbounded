using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new EnemyAttack Attack Move", menuName = "System/Enemy/State/Attack/AttackMove")]
public class EnemyAttackAttackMove : EnemyAttackSOBase
{
    // Class to emulate both chasing and attack behaviour.
    // When in this state, enemy will always attack player (regardless of distance)
    // and move in specified behaviour.

    [SerializeField] private EnemyChaseSOBase chase;


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
        base.DoFrameUpdateLogic();
        chase.DoFrameUpdateLogic();

        if (enemyAIComponent.AggroTarget == null) return;

        Transform targetTransform = enemyAIComponent.AggroTarget.transform;

        enemyAIComponent.InvokeAttackInput(KeyCode.K, new AttackSpawnInfo(targetTransform.position));
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
