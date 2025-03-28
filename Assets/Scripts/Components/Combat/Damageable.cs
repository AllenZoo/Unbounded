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
    [SerializeField] private LocalEventHandler localEventHandler;
    [SerializeField] private StatComponent stat;

    [Tooltip("Attacks targetting this entitytype will be able to damage it.")]
    [SerializeField] private EntityType entityType;

    // Refers to dot attacks that the Damageable is currently taking damage from.
    public EntityType EntityType { get { return entityType; } }

    // Can take damage if true. (Checked by Damageable component to see if hit should register damage)
    public bool isDamageable = true;

    // Can be hit if true. (Checked by Attack component to see if OnHit should trigger)
    public bool isHittable = true;

    private List<Attack> dotAttacks;

    private void Awake()
    {
        Assert.IsNotNull(stat, "Damageable object needs a stat component to calculate final damage.");
        
        // Check that collider2d on object with this script is a trigger.
        Assert.IsTrue(GetComponent<Collider2D>().isTrigger, "Collider2D needs to be a trigger");
        
        // Set collider2d to be a trigger.
        GetComponent<Collider2D>().isTrigger = true;

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                    "] with root object [" + gameObject.transform.root.name + "] for Damageable.cs");
                return;
            }
        }

        LocalEventBinding<OnStatChangeEvent> statModResBinding = new LocalEventBinding<OnStatChangeEvent>(CheckDeath);
        localEventHandler.Register(statModResBinding);
    }

    // Damage needs to be > 0
    public void TakeDamage(float damage)
    {
        if (!isDamageable)
        {
            Debug.Log("Target is currently not damageable!");
            return;
        }

        float calculatedDamage = CalculateDamage(damage);

        if (calculatedDamage <= 0)
        {
            Debug.Log("Damage after stat processing needs to be > 0");
            return;
        }

        localEventHandler.Call(new OnDamagedEvent { damage = calculatedDamage });
    }

    public void TakeDamageOverTime(Attack attack, float damage)
    {
        dotAttacks.Add(attack);
        StartCoroutine(DamageOverTime(attack, damage));
    }

    private IEnumerator DamageOverTime(Attack attack, float damage)
    {
        float total_duration = 0;
        while (dotAttacks.Contains(attack))
        {
            if (total_duration >= attack.AttackData.dotDuration)
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
    private float CalculateDamage(float damage)
    {
        // Increase this to make damage reudction growth slower.
        float growthScale = 8f;

        // Make damage reduction grow at a logarithmic rate. (This function looks good on desmos)
        float damageReduction = Mathf.Log10(stat.defense/growthScale + 1f) / 2f;

        float damageTaken = damage * (1f - damageReduction);

        // Round down to whole number.
        damageTaken = Mathf.Floor(damageTaken);

        return damageTaken;
    }

    private void CheckDeath(OnStatChangeEvent e)
    {
        if (e.statComponent.health <= 0)
        {
            localEventHandler.Call(new OnDeathEvent { });
            // Disable hittable so it can't be hit anymore.
            isHittable = false;
        }
    }
}
