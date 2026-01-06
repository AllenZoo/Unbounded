using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that wraps multiple different kinds of IAttackers so that they can all 'attack' at once.
/// 
/// Note: CD is overwritten by this class. (??, maybe not)
/// 
/// TODO: implement for chillcrusher rage phase.
/// </summary>
public class AttackerWrapper : IAttacker
{
    public List<IAttacker> attackers;

    // TODO: we shoudln't really use this properties.. maybe just leave it here as is for now, we can refactor later :) (+1000%)
    public AttackerData AttackerData { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public AttackData AttackData { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Attack(KeyCode keyCode, AttackContext attackContext)
    {
        foreach (var attacker in attackers)
        {
            if (CheckAttackerStatus(attacker))
            {
                // TODO: figure out how to track each indiviual cooldown.
                attacker.Attack(keyCode, attackContext);
            }
        }
    }

    public void StopAttack()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Always returns true, CanAttack of each individual attacker will be handled in the main Attack() function.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public bool CanAttack()
    {
        return true;
    }

    public IAttacker DeepClone()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Similar to CanAttack, individual GetChargeUp willl be handled in main Attack() function.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public float GetChargeUp()
    {
        return 0f;
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Similar to CanAttack, individual Cooldowns willl be handled in main Attack() function.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public float GetCooldown()
    {
        return 0f;
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Considered initialized as long as we have attackers wrapped.
    /// </summary>
    /// <returns></returns>
    public bool IsInitialized()
    {
        return attackers.Count > 0;
    }

    /// <summary>
    /// Checks if attacker is ready to attack.
    /// </summary>
    /// <param name="attacker"></param>
    /// <returns></returns>
    private bool CheckAttackerStatus(IAttacker attacker)
    {
        return false; //stub
    }
}
