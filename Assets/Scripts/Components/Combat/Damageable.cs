using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

/// <summary>
/// TODO: split logic betweeen DamageableComponent and Damageable. Refer to how AttackComponent and Attack work.
/// </summary>

[RequireComponent(typeof(Collider2D))]
public class Damageable : MonoBehaviour
{
    [SerializeField] private LocalEventHandler leh;
    [SerializeField] private StatComponent stat;

    [Tooltip("Attacks targetting this entitytype will be able to damage it.")]
    [SerializeField] private EntityType entityType;

    // Refers to dot attacks that the Damageable is currently taking damage from.
    public EntityType EntityType { get { return entityType; } }

    // Can take damage if true. (Checked by Damageable component to see if hit should register damage)
    public bool isDamageable = true;

    // Can be hit if true. (Checked by Attack component to see if OnHit should trigger)
    public bool isHittable = true;

    private List<IAttack> dotAttacks;

    private void Awake()
    {
        Assert.IsNotNull(stat, "Damageable object needs a stat component to calculate final damage.");
        
        // Check that collider2d on object with this script is a trigger.
        Assert.IsTrue(GetComponent<Collider2D>().isTrigger, "Collider2D needs to be a trigger");
        
        // Set collider2d to be a trigger.
        GetComponent<Collider2D>().isTrigger = true;

        leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);

        LocalEventBinding<OnStatChangeEvent> statModResBinding = new LocalEventBinding<OnStatChangeEvent>(CheckDeath);
        leh.Register(statModResBinding);
    }

    // Damage needs to be > 0
    public void TakeDamage(float damage)
    {
        TakeDamage(damage, 0);
    }
    public void TakeDamage(float damage, double percentageDamageIncrease)
    {
        Debug.Log($"Percentage Damage Increase is [{percentageDamageIncrease}]");

        if (!isDamageable)
        {
            Debug.Log("Target is currently not damageable!");
            return;
        }


        float calculatedDamage = CalculateDamage(damage, percentageDamageIncrease);

        if (calculatedDamage <= 0)
        {
            Debug.Log("Damage after stat processing needs to be > 0");
            return;
        }

        leh.Call(new OnDamagedEvent { damage = calculatedDamage });
    }


    public void TakeDamageOverTime(IAttack attack, float damage)
    {
        dotAttacks.Add(attack);
        StartCoroutine(DamageOverTime(attack, damage));
    }

    private IEnumerator DamageOverTime(IAttack attack, float damage)
    {
        float total_duration = 0;
        while (dotAttacks.Contains(attack))
        {
            if (total_duration >= attack.AttackData.DotDuration)
            {
                dotAttacks.Remove(attack);
                yield break;
            }

            TakeDamage(damage);
            total_duration += 1f;
            yield return new WaitForSeconds(1f);
        }
    }

    // Calculate damage based on damage amount and damage reduction.
    // At 5 defense, reduce about 10% of damage.
    private float CalculateDamage(float damage, double percentageDamageIncrease)
    {
        // Increase this to make damage reudction growth slower.
        float growthScale = 8f;

        // Make damage reduction grow at a logarithmic rate. (This function looks good on desmos)
        float damageReduction = Mathf.Log10(stat.StatContainer.Defense/growthScale + 1f) / 2f;

        float damageTaken = damage * (1f - damageReduction);

        // Apply Modifier (percentageDamageIncrease)
        float totalDamage = (float)(damageTaken + damageTaken * percentageDamageIncrease / 100);

        // Round down to whole number.
        totalDamage = Mathf.Floor(totalDamage);

        return totalDamage;
    }

    private void CheckDeath(OnStatChangeEvent e)
    {
        if (e.statComponent.StatContainer.Health <= 0)
        {
            leh.Call(new OnDeathEvent { });
            // Disable hittable so it can't be hit anymore.
            isHittable = false;
        }
    }
}
