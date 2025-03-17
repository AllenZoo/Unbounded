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
    [SerializeField] private float idealDist = 5f;

    [Tooltip("The extra *padding* allowed for actual dist to be away from idealDist before directions will change. Should enhance smoothness.")]
    [SerializeField] private float idealDistBuffer = 1f;

    [SerializeField] private bool debugMode = false;



    #region Class Variables
    private bool movingTorwards = false; // to toggle and used with idealDistBuffer to keep track of whether enemy should move torward or away from the player.
    #endregion

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

        if (enemyAIComponent.AggroTarget == null) return;

        // Track the target
        tracker.enabled = true;
        tracker.Track(enemyAIComponent.AggroTarget);

        Transform targetTransform = enemyAIComponent.AggroTarget.transform;
        Transform thisTransform = enemyObject.transform;

        // If debug mode is on, we display raycasts. Blue lines mean out of ideal dist. Yellow line means in ideal dist.
        if (debugMode)
        {
            // Draw lines in a full circle (360 degrees)
            int lineCount = 12;
            for (int i = 0; i < lineCount; i++)
            {
                // Calculate the angle for this line
                float angle = i * (360f / lineCount) * Mathf.Deg2Rad;

                // Calculate the direction vector for this angle
                Vector3 direction = new Vector3(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle),
                    0
                );

                // Draw the ray from the object's position in the calculated direction
                Debug.DrawRay(thisTransform.position, direction.normalized * (idealDist + idealDistBuffer), Color.yellow);
                Debug.DrawRay(thisTransform.position, direction.normalized * (idealDist - idealDistBuffer), Color.blue);
            }

            Vector3 dirDebug = contextSteerer.GetDirTorwards(targetTransform.position, feetTransform.position);
            // Draws lines for visualizing the ideal dist. More direct.
            Debug.DrawRay(thisTransform.position, dirDebug.normalized * (idealDist + idealDistBuffer), Color.yellow);
            Debug.DrawRay(thisTransform.position, dirDebug.normalized * (idealDist - idealDistBuffer), Color.blue);
        }

        // This is to prevent the enemy from stuttering and updating it's movement direction
        // too frequently
        timer -= Time.deltaTime;
        if (timer >= 0f)
        {
            return;
        }

        float dist = Vector2.Distance(thisTransform.position, targetTransform.position);


        // Check if we are too CLOSE.
        if (dist < idealDist - idealDistBuffer)
        {
            // We should move away.
            movingTorwards = false;
        }

        // Check if we are too FAR.
        else if (dist > idealDist + idealDistBuffer)
        {
            // We should move closer.
            movingTorwards = true;
        }

        // If we are not too CLOSE or too FAR keep going same direction. AKA. don't change 'movingTorwards' var.
        // Fetch Direction based on movingTorwards.
        Vector2 dir = contextSteerer.GetDirTorwards(tracker.GetLastSeenTargetPos(), feetTransform.position);
        if (!movingTorwards)
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
        movingTorwards = false;
    }

}
