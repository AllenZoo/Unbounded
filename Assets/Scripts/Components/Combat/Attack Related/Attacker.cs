using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Class that encapsulates the Attacker process (TODO: use chatGpt to think of better way to describe this)
/// Similar to a blueprint for creating attack patterns
/// </summary>
[Serializable]
public class Attacker
{
    public AttackerData AttackerData { get { return attackerData; } set { attackerData = value; } }
    public AttackData AttackData { get { return attackData; } set { attackData = value; } }

    [SerializeField] private AttackerData attackerData;
    [SerializeField] private AttackData attackData;

    public Attacker(AttackerData attackerData, AttackData attackData)
    {
        this.attackerData = attackerData;
        this.attackData = attackData;
    }

    // Attacks and starts cooldown at end of attack. If data or data.attackObj is null, then this function
    // does nothing.
    public void Attack(KeyCode keyCode, AttackSpawnInfo info, Transform attackerTransform, List<EntityType> targetTypes, float atkStat, double percentageDamageIncrease)
    {
        if (attackerData == null || attackData == null)
        {
            return;
        }

        // Play sfx here
        AudioManager.PlaySound(attackData.attackSound, 1);

        //Debug.Log($"Attacking with num atks value of [{attackerData.numAttacks}]");
        for (int i = 0; i < attackerData.numAttacks; i++)
        {
            // i = 0, shoot torwards mouse.
            // i = 1, shoot to the right of mouse with angleOffset * 1 away from attack 0.
            // i = 2, shoot to the left of mouse with angleOffset * 1 away from attack 0.
            //float angle = (i+1)/2 * angleOffset;
            float angle = (int) ((i+1)/2) * attackerData.angleOffset;

            // Odd's offset to the right.
            // Even's offset to the left
            if (i % 2 == 1)
            {
                // odd
                angle *= -1;
            }

            Vector3 attackDir = Quaternion.Euler(0, 0, angle) * (info.mousePosition - attackerTransform.position);

            AttackSpawner.SpawnAttack(attackDir, attackerTransform, targetTypes, attackData.attackPfb, this, atkStat, percentageDamageIncrease);
        }
    }
}
