using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyIdleRandomWalk", menuName = "System/Enemy/State/Idle")]
public class EnemyIdleRandomWalk : EnemyIdleSOBase
{

    [SerializeField]
    [Tooltip("Time interval to change movement direction during random movement.")]
    protected float movementTimer = 3f;
    protected float timer;

    public override void Initialize(EnemyAIComponent enemyAIComponent, GameObject enemyObject)
    {
        base.Initialize(enemyAIComponent, enemyObject);
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

        // If the timer reaches or goes below 0, change movement direction
        if (timer <= 0f)
        {
            // Randomly move (randX = -1/0/1, randY = -1/0/1)
            int randX = UnityEngine.Random.Range(-1, 2); // Include 2 to generate numbers from -1 to 1
            int randY = UnityEngine.Random.Range(-1, 2);

            Vector2 dir = new Vector2(randX, randY);

            // Invoke the event to notify listeners about the new motion direction
            enemyAIComponent.InvokeMovementInput(dir);

            // Reset the timer
            timer = movementTimer;
        }
        
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
