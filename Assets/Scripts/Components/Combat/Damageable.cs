using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public class Damageable : MonoBehaviour
{
    [Tooltip("Attacks targetting this entitytype will be able to damage it.")]
    [SerializeField] private EntityType entityType;
    public EntityType EntityType { get { return entityType; } }

    [SerializeField] private StatComponent stat;

    // Refers to dot attacks that the Damageable is currently taking damage from.
    private List<Attack> dotAttacks;

    private void Awake()
    {
        Assert.IsNotNull(stat, "Damageable object needs a stat component to deal damage to");
        
        // Check that collider2d on object with this script is a trigger.
        Assert.IsTrue(GetComponent<Collider2D>().isTrigger, "Collider2D needs to be a trigger");
        
        // Set collider2d to be a trigger.
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void TakeDamage(float damage)
    {
        stat.ModifyStat(new IStatModifier(Stat.HP, -damage));
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
