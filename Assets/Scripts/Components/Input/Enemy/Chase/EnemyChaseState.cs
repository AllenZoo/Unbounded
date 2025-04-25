public class EnemyChaseState : EnemyStateBase
{
    public EnemyChaseState(EnemyAIComponent enemyAIComponent, EnemyStateMachine stateMachine) : base(enemyAIComponent, stateMachine)
    {

    }

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();
        enemyAIComponent.EnemyChaseBaseInstance.DoAnimationTriggerEventLogic();
    }

    public override void EnterState()
    {
        base.EnterState();
        enemyAIComponent.EnemyChaseBaseInstance.DoEnterLogic();
    }

    public override void ExitState()
    {
        base.ExitState();
        enemyAIComponent.EnemyChaseBaseInstance.DoExitLogic();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();
        enemyAIComponent.EnemyChaseBaseInstance.DoFrameUpdateLogic();
    }


    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        enemyAIComponent.EnemyChaseBaseInstance.DoPhysicsUpdateLogic();
    }
}
