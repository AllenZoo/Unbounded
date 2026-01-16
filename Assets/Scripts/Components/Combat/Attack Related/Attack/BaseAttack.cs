using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public abstract class BaseAttack<T> : IAttack where T : AttackData
{
    public AttackData AttackData { get => attackData; set => attackData = (T)value; }
    [Required, OdinSerialize] protected T attackData;

    /// <summary>
    /// The the atk stat attached to Attack. Boosts the base damage of said attack.
    /// Generally the cumulation of weapon stats + player stats after modifiers applied for each.
    /// </summary>
    [SerializeField, ReadOnly] protected float atkStat = 0;

    /// <summary>
    /// Damage modifier to apply to final calculated damage.
    /// For example after Attack.Damage - Damageable.Defense = TrueDamage
    /// We apply % modifier to TrueDamage: TrueDamage + TrueDamage * % modifier.
    /// </summary>
    [SerializeField, ReadOnly] protected double percentageDamageIncrease = 0;

    public bool Hit(Damageable hit, Transform hitMaker)
    {
        float calculatedDamage = CalculateDamage(attackData.BaseDamage, atkStat);

        // Damage the target.
        if (attackData.IsDOT)
        {
            hit.TakeDamageOverTime(this, calculatedDamage);
            return true;
        }

        hit.TakeDamage(calculatedDamage, percentageDamageIncrease);

        // Knockback the target if:
        //      - attack has knockback
        //      - target is knockbackable
        if (attackData.BaseKnockback > 0)
        {
            Knockbackable kb = hit.GetComponent<Knockbackable>();
            if (kb != null)
            {
                kb.Knockback(hit.transform.position - hitMaker.position, attackData.BaseKnockback, attackData.BaseStunDuration);
            }
        }
        return true;
    }
    public abstract void OnLand(AttackComponent ac);
    public abstract void OnLaunch(AttackComponent ac);
    public abstract void Reset(AttackComponent ac);
    public void SetModifiers(float atkStat, double percentageDamageIncrease)
    {
        this.atkStat = atkStat;
        this.percentageDamageIncrease = percentageDamageIncrease;
    }
        

    // Calculates the damage of the attack while also taking into account the attacker's stats.
    // TODO: move this logic to a util class alongside with the damage calculation formula in Damageable.
    // Note: Copied from Attack.cs
    private float CalculateDamage(float baseDamage, float atkStat)
    {
        float calculatedDMG = baseDamage + atkStat;
        return calculatedDMG;

    }
}
