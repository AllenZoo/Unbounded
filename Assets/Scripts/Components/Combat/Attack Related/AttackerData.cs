using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackerData
{
    [Tooltip("The prefab to spawn when attacking.")]
    public GameObject attackObj;

    [Tooltip("Number of attacks to spawn when attacking.")]
    public int numAttacks = 1;

    [Tooltip("Angle offset for between each attack spawned.")]
    public float angleOffset = 0f;

    [Tooltip("Cooldown of attacker to launch attacks.")]
    public float cooldown = 0.5f;

    public AttackerData(Attack attackObj, int numAttacks, float angleOffset, float cooldown)
    {
        this.attackObj = attackObj.gameObject;
        this.numAttacks = numAttacks;
        this.angleOffset = angleOffset;
        this.cooldown = cooldown;
    }

    public AttackerData(GameObject attackObj, int numAttacks, float angleOffset, float cooldown)
    {
        this.attackObj = attackObj;
        this.numAttacks = numAttacks;
        this.angleOffset = angleOffset;
        this.cooldown = cooldown;
    }

    public AttackerData()
    {
        this.attackObj = null;
        this.numAttacks = 1;
        this.angleOffset = 0f;
        this.cooldown = 0.5f;
    }


    // Copy function to create a deep copy of the attacker data.
    public AttackerData Copy()
    {
        AttackerData copy = new AttackerData();
        copy.attackObj = this.attackObj;
        copy.numAttacks = this.numAttacks;
        copy.angleOffset = this.angleOffset;
        copy.cooldown = this.cooldown;
        return copy;
    }
}
