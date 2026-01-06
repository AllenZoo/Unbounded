using System;
using UnityEngine;

/// <summary>
/// Handles cooldown and attack readiness of given attacker.
/// </summary>
[Serializable]
public class AttackSlot
{
    public IAttacker Attacker;
    public float nextReadyTime;

    public bool IsReady => Time.time >= nextReadyTime;

    public AttackSlot(IAttacker attacker)
    {
        Attacker = attacker; 
        nextReadyTime = Time.time;
    }

    public AttackSlot(IAttacker attacker, float nextReadyTime)
    {
        Attacker = attacker;
        this.nextReadyTime = nextReadyTime;
    }

    public void Trigger(KeyCode k, AttackContext ac)
    {
        Attacker.Attack(k, ac);
        nextReadyTime = Time.time + Attacker.GetCooldown();
    }

    public void StopTrigger()
    {
        Attacker.StopAttack();
    }
}
