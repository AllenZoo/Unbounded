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

    public bool IsReady => (Time.time >= nextReadyTime) && Attacker != null;

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
        if (Attacker == null)
        {
            Debug.LogError("Attacker is null!");
            return;
        }

        Attacker.Attack(k, ac);
        nextReadyTime = Time.time + Attacker.GetCooldown();
    }

    public void StopTrigger()
    {
        if (Attacker == null)
        {
            Debug.LogError("Attacker is null!");
            return;
        }
        Attacker.StopAttack();
    }
}
