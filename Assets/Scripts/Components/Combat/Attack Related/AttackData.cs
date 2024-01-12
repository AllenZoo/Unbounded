using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackData
{
    // TODO: Split these fields into different component classes. eg. DOT component, AOE component, etc.
    public string attackName = "Attack";
    public float damage = 5f;
    public float duration = 0.5f;
    public float initialSpeed = 0f;

    [Tooltip("time it takes to charge up attack.")]
    public float chargeUp = 0f;
    public float knockback = 0f;
    public float stunDuration = 1f;

    [Tooltip("If true, the attack will hit all targets in the collider. If false, it will only hit the first target.")]
    public bool isAOE = false;

    [Tooltip("If true, the attack will be able to hit the same target multiple times.")]
    public bool canRepeat = false;

    [Tooltip("If true, the attack will deal damage over time.")]
    public bool isDOT = false;
    public float dotDuration = 5f;

    [Tooltip("If true, the attack will pierce through targets.")]
    public bool isPiercing = false;

    [Tooltip("If true, the attack will last until duration is over.")]
    public bool lastsUntilDuration = false;

    [Header("For rendering")]
    [Tooltip("Data for rendering the attack.")]
    public float rotOffset;
    public Sprite attackSprite;
}
