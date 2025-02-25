using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyChase Kite Target", menuName = "System/Enemy/State/Chase/KiteTarget")]
public class EnemyChaseKiteTarget : EnemyChaseSOBase
{
    [Tooltip("Time interval to change movement direction during chase movement. ")]
    [SerializeField] protected float timeStagger = 0.2f;
    private float timer = 0;

    [Tooltip("The ideal distance/space to keep from the target")]
    [SerializeField] private float idealDist = 1f;

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

        tracker.enabled = true;
        tracker.Track(enemyAIComponent.AggroTarget);

        // This is to prevent the enemy from stuttering and updating it's movement direction
        // too frequently
        if (timer >= 0f)
        {
            return;
        }

        Transform targetTransform = enemyAIComponent.AggroTarget.transform;
        Transform thisTransform = enemyObject.transform;

        float dist = Vector2.Distance(thisTransform.position, targetTransform.position);

        // Initial direction to move torwards target
        Vector2 dir = contextSteerer.GetDirTorwards(tracker.GetLastSeenTargetPos(), feetTransform.position);

        if (dist < idealDist)
        {
            // Move away from the target
            dir = contextSteerer.GetDirAway(tracker.GetLastSeenTargetPos(), feetTransform.position);
        }

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
