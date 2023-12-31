using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(StatComponent))]
[RequireComponent (typeof(Rigidbody2D))]
public class Damageable : MonoBehaviour
{
    private StatComponent stat;

    // Refers to dot attacks that the Damageable is currently taking damage from.
    private List<Attack> dotAttacks;

    private void Awake()
    {
        stat = GetComponent<StatComponent>();
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
