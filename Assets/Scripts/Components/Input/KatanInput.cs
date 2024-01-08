using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Input controller for the boss 'Katan'
public class KatanInput : EnemyAIComponent
{
    public event Action<int> OnPhaseChange;

    [Header("For debugging purposes.")]
    [SerializeField] private int phase = 0;

    private new void Start()
    {
        base.Start();
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
        // Debug.Log("Katan in phase 0!");

        // Melee move torwards player
        base.Targetted_Move(aggroTarget, attackRange);

    }

    private void Phase1()
    {
        // Debug.Log("Katan in phase 1!");

        // Ranged move away from player
        base.Targetted_Ranged_Move(aggroTarget, minDist, attackRange);
    }
}
