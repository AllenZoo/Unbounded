using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public class Damageable : MonoBehaviour
{
    // <Damage>
    public event Action<float> OnDamage;
    public event Action OnDeath;
    
    [Tooltip("Attacks targetting this entitytype will be able to damage it.")]
    [SerializeField] private EntityType entityType;
    [SerializeField] private StatComponent stat;

    // Refers to dot attacks that the Damageable is currently taking damage from.
    public EntityType EntityType { get { return entityType; } }

    // Can take damage if true. (Checked by Damageable component to see if hit should register damage)
    public bool isDamageable = true;

    // Can be hit if true. (Checked by Attack component to see if OnHit should trigger)
    public bool isHittable = true;

    private List<Attack> dotAttacks;

    private void Awake()
    {
        Assert.IsNotNull(stat, "Damageable object needs a stat component to deal damage to");
        
        // Check that collider2d on object with this script is a trigger.
        Assert.IsTrue(GetComponent<Collider2D>().isTrigger, "Collider2D needs to be a trigger");
        
        // Set collider2d to be a trigger.
        GetComponent<Collider2D>().isTrigger = true;
    }

    // Damage needs to be > 0
    public void TakeDamage(float damage)
    {
        if (!isDamageable)
        {
            Debug.Log("Target is currently not damageable!");
            return;
        }

        if (damage <= 0)
        {
            Debug.Log("Damage needs to be > 0");
            return;
        }

        stat.ModifyStat(new IStatModifier(Stat.HP, -damage));

        if (stat.GetCurStat(Stat.HP) <= 0)
        {
            OnDeath?.Invoke();

            // Disable hittable so it can't be hit anymore.
            isHittable = false;
        } else
        {
            OnDamage?.Invoke(damage);
        }
    }

    public void TakeDamageOverTime(Attack attack)
    {
        dotAttacks.Add(attack);
        StartCoroutine(DamageOverTime(attack));
    }

    private IEnumerator DamageOverTime(Attack attack)
    {
        float total_duration = 0;
        while (dotAttacks.Contains(attack))
        {
            if (total_duration >= attack.DotDuration)
            {
                dotAttacks.Remove(attack);
                yield break;
            }

            TakeDamage(attack.Damage);
            total_duration += 1f;
            yield return new WaitForSeconds(1f);
        }
    }
}
