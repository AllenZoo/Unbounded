public class EnemyIdleState : EnemyStateBase
{
    public EnemyIdleState(EnemyAIComponent enemyAIComponent, EnemyStateMachine stateMachine) : base(enemyAIComponent, stateMachine)
    {

    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        enemyAIComponent.EnemyIdleBaseInstance.DoAnimationTriggerEventLogic();
    }

    public override void EnterState()
    {
        base.EnterState();
        enemyAIComponent.EnemyIdleBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        enemyAIComponent.EnemyIdleBaseInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemyAIComponent.EnemyIdleBaseInstance.DoFrameUpdateLogic();
    }


    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemyAIComponent.EnemyIdleBaseInstance.DoPhysicsUpdateLogic();
    }
}
