using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyChase Dont Move Behaviour", menuName = "System/Enemy/State/Chase/DontMove")]
public class EnemyChaseDontMove : EnemyChaseSOBase
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

    public override void DoFrameUpdateLogic(bool stateChange)
    {
        base.DoFrameUpdateLogic(stateChange);
        enemyAIComponent.InvokeMovementInput(Vector2.zero);
    }


    public override void DoPhysicsUpdateLogic()
    {
        base.DoPhysicsUpdateLogic();
    }

    public override void Initialize(EnemyAIComponent enemyAIComponent, GameObject enemyObject, ContextSteerer contextSteerer, ObjectTracker tracker, Transform feetTransform)
    {
        base.Initialize(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}
