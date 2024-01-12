using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackData
{
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

    [Header("For DOT")]
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

    [Header("Dynamic Data that set by Attacker")]
    public float attackerAtkStat;

    // Copy function to create a deep copy of the attack data.
    public AttackData Copy()
    {
        AttackData copy = new AttackData();
        copy.attackName = this.attackName;
        copy.damage = this.damage;
        copy.duration = this.duration;
        copy.initialSpeed = this.initialSpeed;
        copy.chargeUp = this.chargeUp;
        copy.knockback = this.knockback;
        copy.stunDuration = this.stunDuration;
        copy.isAOE = this.isAOE;
        copy.canRepeat = this.canRepeat;
        copy.isDOT = this.isDOT;
        copy.dotDuration = this.dotDuration;
        copy.isPiercing = this.isPiercing;
        copy.lastsUntilDuration = this.lastsUntilDuration;
        copy.rotOffset = this.rotOffset;
        copy.attackSprite = this.attackSprite;
        copy.attackerAtkStat = this.attackerAtkStat;
        return copy;
    }
}
