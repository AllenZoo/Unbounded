public class EnemyAttackState : EnemyStateBase
{
    public EnemyAttackState(EnemyAIComponent enemyAIComponent, EnemyStateMachine stateMachine) : base(enemyAIComponent, stateMachine)
    {

    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        enemyAIComponent.EnemyAttackBaseInstance.DoAnimationTriggerEventLogic();
    }

    public override void EnterState()
    {
        base.EnterState();
        enemyAIComponent.EnemyAttackBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        enemyAIComponent.EnemyAttackBaseInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemyAIComponent.EnemyAttackBaseInstance.DoFrameUpdateLogic();
    }


    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemyAIComponent.EnemyAttackBaseInstance.DoPhysicsUpdateLogic();
    }
}
