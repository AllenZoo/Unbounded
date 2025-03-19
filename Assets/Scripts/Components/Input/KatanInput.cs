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
        base.Update();
    }
}
