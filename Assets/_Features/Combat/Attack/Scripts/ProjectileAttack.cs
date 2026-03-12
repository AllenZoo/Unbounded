using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
public class ProjectileAttack: IAttack
{
    public event Action<Damageable> OnHit;

    public AttackData AttackData { get => attackData; set => attackData = value; }

    [Required, OdinSerialize]
    private AttackData attackData;

    // For Debugging
    [SerializeField, ReadOnly] private AttackDamageModifiers adm = new AttackDamageModifiers();

    /// <summary>
    /// Algorithm for handling what happens when a projectile attack hits a damageable target.
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="hitMaker"></param>
    /// <param name="ac"></param>
    /// <returns></returns>
    public bool Hit(Damageable hit, Transform attackObj, AttackComponent ac, Transform attackSource)
    {
        float calculatedDamage = CalculateDamage(attackData.BaseDamage, ac.AttackerContext.AtkStat);

        // Damage the target.
        if (attackData.IsDOT)
        {
            hit.TakeDamageOverTime(this, calculatedDamage, attackSource);
            return true;
        }

        hit.TakeDamage(calculatedDamage, ac.AttackerContext.PercentageDamageIncrease, attackSource);
        
        // Knockback the target if:
        //      - attack has knockback
        //      - target is knockbackable
        if (attackData.BaseKnockback > 0)
        {
            Knockbackable kb = hit.GetComponent<Knockbackable>();
            if (kb != null)
            {
                kb.Knockback(hit.transform.position - attackObj.position, attackData.BaseKnockback, attackData.BaseStunDuration);
            }
        }
        return true;
    }

    public void OnLaunch(AttackComponent ac)
    {
        // Do nothing for basic projectile attack.

        // Initialize base states
        if (adm == null)
        {
            adm = new AttackDamageModifiers();
        }
        adm.AtkStat = ac.AttackerContext.AtkStat;
        adm.PercentageDamageIncrease = ac.AttackerContext.PercentageDamageIncrease;
    }

    public void OnLand(AttackComponent ac)
    {
        // Do nothing for basic projectile attack.
        //adm = null;

    }
    public void SetModifiers(float atkStat, double percentageDamageIncrease)
    {
        //this.atkStat = atkStat;
        //this.percentageDamageIncrease = percentageDamageIncrease;
    }

    public void Reset(AttackComponent ac)
    {
        // Do nothing for basic projectile attack.
    }



    // Calculates the damage of the attack while also taking into account the attacker's stats.
    // TODO: move this logic to a util class alongside with the damage calculation formula in Damageable.
    private float CalculateDamage(float baseDamage, float atkStat)
    {
        float calculatedDMG = baseDamage + atkStat;
        return calculatedDMG;

    }
}

