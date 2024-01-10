using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Input controller for the boss 'Katan'
public class KatanInput : EnemyAIComponent
{
    [Header("Katan Inputs")]
    [SerializeField] private Transform centerTransform;

    [Tooltip("Maximum distance away from centerTransform that Katan can move during certain phases.")]
    [SerializeField] private float maxDistAwayFromCenter = 5f;

    [SerializeField] private RingAttack ringAttack;

    // Needed as Katan has phases.
    [SerializeField] private PhaseManager phaseManager;

    private delegate void PhaseAction();
    private Dictionary<int, PhaseAction> phaseMap;

    private new void Awake()
    {
        base.Awake();
        if (ringAttack == null)
        {
            ringAttack = GetComponentInChildren<RingAttack>();
        }
        Assert.IsNotNull(ringAttack, "Katan needs a ring attack obj!");

        // Init Phases
        phaseMap = new Dictionary<int, PhaseAction>
        {
            { 0, Phase0 },
            { 1, Phase1 },
            { 2, Phase2 }
        };
    }

    private new void Start()
    {
        base.Start();
        if (centerTransform == null)
        {
            centerTransform = gameObject.transform;
        }
    }

    protected new void Update()
    {
        // Count down the timer
        base.timer -= Time.deltaTime;

        if (aggroTarget != null)
        {
            // Aggroed
            tracker.enabled = true;
            tracker.Track(aggroTarget);

            // Check if phase behaviour is in dictionary
            if (phaseMap.ContainsKey(phaseManager.Phase))
            {
                // Run phase
                phaseMap[phaseManager.Phase]();
            }
        }
        else
        {
            tracker.enabled = false;
            Random_Move();
        }
    }

    // Ring rush.
    private void Phase0()
    {
        // Follow + Kite so that player just gets hit by ring attack.
        base.KiteTarget(aggroTarget, ringAttack.Radius/2);

        ringAttack.Toggle(true);
    }

    private void Phase1()
    {
        // Kite
        base.KiteTarget(aggroTarget, minDist);
        base.ReadyAttack(aggroTarget, attackRange);

        ringAttack.Toggle(false);
    }

    // Move close to the player but stay within a maxDist of centerPos.
    private void Phase2()
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
            base.Follow(aggroTarget);
        }

        base.ReadyAttack(aggroTarget, attackRange);

        ringAttack.Toggle(true);
    }
}
