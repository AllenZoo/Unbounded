using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Behavioural class that encapsulate attack hit.
/// Holds both static and dynamic attack data.
/// </summary>
[Serializable]
public class Attack 
{
    public event Action<Damageable> OnHit;

    public AttackData AttackData
    {
        get { return attackData; }
        private set { }
    }

    [SerializeField, Required]
    private AttackData attackData;

    /// <summary>
    /// The the atk stat attached to Attack. Boosts the base damage of said attack.
    /// Generally the cumulation of weapon stats + player stats after modifiers applied for each.
    /// </summary>
    private float atkStat = 0;

    public Attack()
    {
        this.atkStat = 0f;
    }
    public Attack(float atkStat)
    {
        this.atkStat = atkStat;
    }


    // Logic to determine what happens when the attack hits a target.
    // TODO: maybe refactor what passes througuh Hit (probably thing that got hit --> Hittable?)
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="hitObject"></param>

    public void Hit(Damageable hit, Transform hitMaker)
    {
        float calculatedDamage = CalculateDamage(attackData.baseDamagee, atkStat);

        // Damage the target.
        if (attackData.isDOT)
        {
            hit.TakeDamageOverTime(this, calculatedDamage);
            return;
        }

        hit.TakeDamage(calculatedDamage);
        
        // Knockback the target if:
        //      - attack has knockback
        //      - target is knockbackable
        if (attackData.baseKnockback > 0)
        {
            Knockbackable kb = hit.GetComponent<Knockbackable>();
            if (kb != null)
            {
                kb.Knockback(hit.transform.position - hitMaker.position, attackData.baseKnockback, attackData.baseStunDuration);
            }
        }


        // Resets the attack if conditions are met.
        if (!attackData.isAOE && !attackData.isPiercing && !attackData.lastsUntilDuration)
        {
            // TODO: movee this logic into the component.
            // Destroy the attack object. (or set inactive if we want to reuse it)
            return;
        }

    }

    public void SetAtkStat(float atkStat)
    {
        this.atkStat = atkStat;
    }

    // Calculates the damage of the attack while also taking into account the attacker's stats.
    // TODO: move this logic to a util class alongside with the damage calculation formula in Damageable.
    private float CalculateDamage(float baseDamage, float atkStat)
    {
        float calculatedDMG = baseDamage + atkStat;
        return calculatedDMG;

    }
}
