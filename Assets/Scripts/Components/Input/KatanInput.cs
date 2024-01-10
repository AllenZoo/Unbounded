using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Input controller for the boss 'Katan'
public class KatanInput : EnemyAIComponent
{
    public event Action<int> OnPhaseChange;

    [Header("Katan Inputs")]
    [SerializeField] private Transform centerTransform;

    [Tooltip("Maximum distance away from centerTransform that Katan can move during certain phases.")]
    [SerializeField] private float maxDistAwayFromCenter = 5f;

    [SerializeField] private RingAttack ringAttack;

    // Needed as Katan has phases.
    [SerializeField] private PhaseManager phaseManager;

    [Header("For debugging purposes.")]
    [SerializeField] private int phase = 0;

    private delegate void Phase();
    private Dictionary<int, Phase> phaseMap;

    private new void Awake()
    {
        base.Awake();
        if (ringAttack == null)
        {
            ringAttack = GetComponentInChildren<RingAttack>();
        }
        Assert.IsNotNull(ringAttack, "Katan needs a ring attack obj!");

        // Init Phases
        phaseMap = new Dictionary<int, Phase>
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
        OnPhaseChange?.Invoke(phase);
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

            // Run phase
            phaseMap[phase]();
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
        // Follow
        //base.Follow(aggroTarget);
        base.KiteTarget(aggroTarget, ringAttack.Radius/2);

        // base.ReadyAttack(aggroTarget, attackRange);

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
