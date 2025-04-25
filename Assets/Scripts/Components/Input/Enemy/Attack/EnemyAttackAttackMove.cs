using UnityEngine;


[CreateAssetMenu(fileName = "new EnemyAttack Attack Move", menuName = "System/Enemy/State/Attack/AttackMove")]
public class EnemyAttackAttackMove : EnemyAttackSOBase
{
    // Class to emulate both chasing and attack behaviour.
    // When in this state, enemy will always attack player (regardless of distance)
    // and move in specified behaviour.

    [SerializeField] private EnemyChaseSOBase chase;

    private EnemyChaseSOBase chaseInstance;

    public override void Initialize(EnemyAIComponent enemyAIComponent, GameObject enemyObject, ContextSteerer contextSteerer, ObjectTracker tracker, Transform feetTransform)
    {
        chaseInstance = Instantiate(chase);
        base.Initialize(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);
        chaseInstance.Initialize(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);
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
        chaseInstance.DoFrameUpdateLogic(false);

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
