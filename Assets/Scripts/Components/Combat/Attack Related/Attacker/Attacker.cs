using Sirenix.Serialization;
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
public class Attacker: IAttacker
{
    public AttackerData AttackerData { get { return attackerData; } set { attackerData = value; } }
    public AttackData AttackData { get { return attackData; } set { attackData = value; } }

    [OdinSerialize] private AttackerData attackerData;
    [OdinSerialize] private AttackData attackData; // TODO: maybe move this elsewhere. (add as parameter to main Attack function)

    public Attacker() { }

    public Attacker(AttackerData attackerData, AttackData attackData)
    {
        this.attackerData = attackerData;
        this.attackData = attackData;
    }

    // Attacks and starts cooldown at end of attack. If data or data.attackObj is null, then this function
    // does nothing.
    public void Attack(KeyCode keyCode, AttackContext ac)
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

            Vector3 attackDir = Quaternion.Euler(0, 0, angle) * (ac.SpawnInfo.mousePosition - ac.AttackerTransform.position);

            AttackSpawner.SpawnAttackInPool(attackDir, ac.AttackerTransform, ac.TargetTypes, attackData.attackPfb, this, ac.AtkStat, ac.PercentageDamageIncrease);
        }
    }

    public void StopAttack()
    {
        // Do nothing for basic attacker.
    }

    public bool IsInitialized()
    {
        return attackerData != null && attackData != null;
    }

    public float GetCooldown()
    {
        return attackerData.cooldown;
    }

    public float GetChargeUp()
    {
        return attackerData.chargeUp;
    }

    public bool CanAttack()
    {
        return true;
    }


    public IAttacker DeepClone()
    {
        AttackerData clonedAttackerData = UnityEngine.Object.Instantiate(attackerData);
        AttackData clonedAttackData = UnityEngine.Object.Instantiate(attackData);
        return new Attacker(clonedAttackerData, clonedAttackData);
    }
}
