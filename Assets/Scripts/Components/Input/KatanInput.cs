using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Input controller for the boss 'Katan'
public class KatanInput : EnemyAIComponent
{
    public event Action<int> OnPhaseChange;

    [SerializeField] private Transform centerTransform;

    [Tooltip("Maximum distance away from centerTransform that Katan can move during certain phases.")]
    [SerializeField] private float maxDistAwayFromCenter = 5f;

    [Header("For debugging purposes.")]
    [SerializeField] private int phase = 0;

    private new void Start()
    {
        base.Start();
        if (centerTransform == null)
        {
            centerTransform = gameObject.transform;
        }
        OnPhaseChange?.Invoke(phase);
    }

    protected new void Update()
    {
        // Count down the timer
        base.timer -= Time.deltaTime;

        // Debug.Log("Aggro target: " + aggroTarget);
        if (aggroTarget != null)
        {
            // Aggroed
            tracker.enabled = true;
            tracker.Track(aggroTarget);
            switch (phase)
            {
                case 0:
                    Phase0();
                    break;
                case 1:
                    Phase1();
                    break;
                default:
                    break;
            }
        }
        else
        {
            tracker.enabled = false;
            Random_Move();
        }
    }

    private void Phase0()
    {
        // Follow
        base.Follow(aggroTarget);
        base.ReadyAttack(aggroTarget, attackRange);

    }

    private void Phase1()
    {
        // Kite
        base.KiteTarget(aggroTarget, minDist);
        base.ReadyAttack(aggroTarget, attackRange);
    }


    // Move close to the player but stay within a maxDist of centerPos.
    private void Phase2(GameObject target)
    {
        float centerDist = Vector2.Distance(transform.position, centerTransform.position);

        // If Katan is too far away from the center, move closer to the center.
        if (centerDist > maxDistAwayFromCenter)
        {
            // Move closer to the center.
            base.Follow(centerTransform.gameObject);
        }
        else
        {
            // Move closer to the player.
            base.Follow(target);
        }

        base.ReadyAttack(target, attackRange);
    }
}
