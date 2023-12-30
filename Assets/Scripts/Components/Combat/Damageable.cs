using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(StatComponent))]
[RequireComponent (typeof(Rigidbody2D))]
public class Damageable : MonoBehaviour
{
    private StatComponent stat;

    private void Awake()
    {
        stat = GetComponent<StatComponent>();
    }

    public void TakeDamage(float damage)
    {
        stat.ModifyStat(new IStatModifier(Stat.HP, -damage));
    }
}
