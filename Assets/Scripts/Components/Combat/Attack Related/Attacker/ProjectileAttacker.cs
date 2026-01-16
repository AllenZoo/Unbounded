using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

// TODO: rename this to ProjectileAttacker

/// <summary>
/// Class that encapsulates the Attacker process (TODO: use chatGpt to think of better way to describe this)
/// Similar to a blueprint for creating attack patterns
/// </summary>
[Serializable]
public class ProjectileAttacker: BaseAttacker<AttackerData>
{
    public override AttackData AttackData { get { return attackData; } set { attackData = value; } }
    [OdinSerialize] private AttackData attackData; // TODO: maybe move this elsewhere. (add as parameter to main Attack function)


    public ProjectileAttacker() { }

    public ProjectileAttacker(AttackerData attackerData, AttackData attackData)
    {
        this.attackerData = attackerData;
        this.attackData = attackData;
    }

    // Attacks. If data or data.attackObj is null, then this function
    // does nothing.
    public override void Attack(KeyCode keyCode, AttackContext ac)
    {
        if (attackerData == null || attackData == null)
        {
            return;
        }

        // Play sfx here
        AudioManager.PlaySound(attackData.AttackSound, 1);

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

            // Vector3 attackDir = Quaternion.Euler(0, 0, angle) * (ac.AttackSpawnInfo.targetPosition - ac.AttackerTransform.position);

            var attackComponent = attackData.AttackPfb?.GetComponent<AttackComponent>();
            if (attackComponent == null) Debug.LogError("Attack Pfb Does not contain Attack Component!");

            var attack = attackComponent.Attack;
            var attackMovement = attackComponent.Movement;

            var amc = new AttackModificationContext
            {
                ObjectScale = 1,
                AttackDuration = attackData.Distance / attackData.InitialSpeed,
                AngleOffset = angle
            };


            AttackSpawner.Spawn(attackData.AttackPfb, attackData, ac, amc, attack, attackMovement);
        }
    }

    public override void StopAttack()
    {
        // Do nothing for basic attacker.
    }

    public override bool IsInitialized()
    {
        return attackerData != null && attackData != null;
    }

    public override float GetCooldown()
    {
        return attackerData.cooldown;
    }

    public override float GetChargeUp()
    {
        return attackerData.chargeUp;
    }

    public bool CanAttack()
    {
        return true;
    }


    public override IAttacker DeepClone()
    {
        AttackerData clonedAttackerData = UnityEngine.Object.Instantiate(attackerData);
        AttackData clonedAttackData = UnityEngine.Object.Instantiate(attackData);
        return new ProjectileAttacker(clonedAttackerData, clonedAttackData);
    }

    #region IAttackNode Implementation
    public override IEnumerable<IAttackNode> GetChildren()
    {
        yield return this;
    }
    #endregion
}
