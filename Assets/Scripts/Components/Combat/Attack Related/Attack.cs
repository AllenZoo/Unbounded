using System;
using UnityEngine;

/// <summary>
/// Behavioural class that encapsulate attack hit. (TODO: use chatGpt to think of better way to describe this)
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

    [SerializeField]
    private AttackData attackData;



    // Logic to determine what happens when the attack hits a target.
    // TODO: maybe refactor what passes througuh Hit (probably thing that got hit --> Hittable?)
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="hitObject"></param>

    public void Hit(Damageable hit, Transform hitMaker)
    {

        // TODO: reimplement
        //float calculatedDamage = CalculateDamage(attackData.baseDamagee, attackerATKStat);
        float calculatedDamage = CalculateDamage(attackData.baseDamagee, 1);


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

    // Calculates the damage of the attack while also taking into account the attacker's stats.
    // TODO: move this logic to a util class alongside with the damage calculation formula in Damageable.
    private float CalculateDamage(float baseDamage, float atkStat)
    {
        float calculatedDMG = baseDamage + atkStat;
        return calculatedDMG;

    }
}
