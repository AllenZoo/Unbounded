using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class BombAttack : IAttack
{
    public AttackData AttackData { get => bombAttackData; set => bombAttackData = (BombAttackData) value; }

    [Required, OdinSerialize] private BombAttackData bombAttackData;

    /// <summary>
    /// The the atk stat attached to Attack. Boosts the base damage of said attack.
    /// Generally the cumulation of weapon stats + player stats after modifiers applied for each.
    /// </summary>
    [SerializeField, ReadOnly] private float atkStat = 0;

    /// <summary>
    /// Damage modifier to apply to final calculated damage.
    /// For example after Attack.Damage - Damageable.Defense = TrueDamage
    /// We apply % modifier to TrueDamage: TrueDamage + TrueDamage * % modifier.
    /// </summary>
    [SerializeField, ReadOnly] private double percentageDamageIncrease = 0;

    public BombAttack() { }

    public bool Hit(Damageable hit, Transform hitMaker)
    {
        float calculatedDamage = CalculateDamage(bombAttackData.baseDamage, atkStat);

        // Damage the target.
        if (bombAttackData.isDOT)
        {
            hit.TakeDamageOverTime(this, calculatedDamage);
            return true;
        }

        hit.TakeDamage(calculatedDamage, percentageDamageIncrease);

        // Knockback the target if:
        //      - attack has knockback
        //      - target is knockbackable
        if (bombAttackData.baseKnockback > 0)
        {
            Knockbackable kb = hit.GetComponent<Knockbackable>();
            if (kb != null)
            {
                kb.Knockback(hit.transform.position - hitMaker.position, bombAttackData.baseKnockback, bombAttackData.baseStunDuration);
            }
        }
        return true;
    }

    public void OnLaunch(AttackComponent ac)
    {
        // Deactivate hit collision until it explodes.
        ac.AttackCollider.enabled = false;
        throw new System.NotImplementedException();

    }

    public void OnLand(AttackComponent ac)
    {
        // Start fuse timer.
        //bombAttackData.fuseTime;
        ac.StartCoroutine(OnLandEnumerator(ac));
    }

    public IEnumerator OnLandEnumerator(AttackComponent ac)
    {
        yield return new WaitForSeconds(bombAttackData.fuseTime);
        Explode(ac);
    }

    public void Explode(AttackComponent ac)
    {
        // Change Sprite
        SpriteRenderer spriteRenderer = ac.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
            spriteRenderer.sprite = bombAttackData.explosionSprite;
            spriteRenderer.enabled = true;
        }

        // Explode and do damage to any entity in radius. (e.g. collider)

        // Option 1: Do this by spawning an explosion attack overtop.
        // Option 2: Allow for Triggers to occur. (picked this one)
        ac.AttackCollider.enabled = true;

    }

    public void Reset(AttackComponent ac)
    {
        // Change Sprite back to original.
        SpriteRenderer spriteRenderer = ac.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
            spriteRenderer.sprite = bombAttackData.initSprite;
            spriteRenderer.enabled = true;
        }
    }

    public void SetModifiers(float atkStat, double percentageDamageIncrease)
    {
        this.atkStat = atkStat;
        this.percentageDamageIncrease = percentageDamageIncrease;
    }


    // Taken from Attack.cs. TODO: eventually refactor this (along with the duplciate code in Attack.cs) to implement more compelx damage system.
    private float CalculateDamage(float baseDamage, float atkStat)
    {
        float calculatedDMG = baseDamage + atkStat;
        return calculatedDMG;

    }
}
