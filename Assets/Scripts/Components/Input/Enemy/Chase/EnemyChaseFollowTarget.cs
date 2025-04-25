using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyChase Follow Target", menuName = "System/Enemy/State/Chase/FollowTarget")]
public class EnemyChaseFollowTarget : EnemyChaseSOBase
{
    [Tooltip("Time interval to change movement direction during chase movement. ")]
    [SerializeField] protected float timeStagger = 0.2f;
    private float timer = 0;


    // TODO:
    // Sight range, etc. for object tracker

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

        if (enemyAIComponent.AggroTarget != null)
        {
            tracker.enabled = true;
            tracker.Track(enemyAIComponent.AggroTarget);
        }

        // This is to prevent the enemy from stuttering and updating it's movement direction
        // too frequently
        timer -= Time.deltaTime;
        if (timer >= 0f)
        {
            return;
        }

        // Use context steering to determine movement direction to best reach target.
        Vector2 dir = contextSteerer.GetDirTorwards(tracker.GetLastSeenTargetPos(), feetTransform.position);

        // Move towards the target
        enemyAIComponent.InvokeMovementInput(dir);

        // Stutter the timer to prevent crazy movement.
        timer = timeStagger;
    }


    public override void DoPhysicsUpdateLogic()
    {
        base.DoPhysicsUpdateLogic();
    }

    public override void Initialize(EnemyAIComponent enemyAIComponent, GameObject enemyObject, ContextSteerer contextSteerer, ObjectTracker tracker, Transform feetTransform)
    {
        base.Initialize(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);
        timer = 0;
    }

    public override void ResetValues()
    {
        base.ResetValues();
        timer = 0;
    }
}
