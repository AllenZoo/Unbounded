using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyIdle StandStill", menuName = "System/Enemy/State/Idle/StandStill")]
public class EnemyIdleStandStill : EnemyIdleSOBase
{
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
        enemyAIComponent.InvokeMovementInput(Vector2.zero);
    }

    public override void DoPhysicsUpdateLogic()
    {
        base.DoPhysicsUpdateLogic();
    }

    public override void Initialize(EnemyAIComponent enemyAIComponent, GameObject enemyObject)
    {
        base.Initialize(enemyAIComponent, enemyObject);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}
