using NUnit.Framework;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that wraps multiple different kinds of IAttackers so that they can all 'attack' at once.
/// </summary>
public class AttackerWrapper : IAttacker, IAttackNode
{
    [OdinSerialize]
    public List<IAttacker> attackers;

    // TODO: we shoudln't really use this properties.. maybe just leave it here as is for now, we can refactor later :) (+1000%)
    public AttackerData AttackerData { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public AttackData AttackData { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Attack(KeyCode keyCode, AttackContext attackContext)
    {
        foreach (var attacker in attackers)
        {
            attacker.Attack(keyCode, attackContext);
        }
    }

    public void StopAttack()
    {
        foreach (var attacker in attackers)
        {
            attacker.StopAttack();
        }
    }


    public IAttacker DeepClone()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Similar to CanAttack, individual GetChargeUp willl be handled in main Attack() function.
    /// </summary>
    /// <returns></returns>
    public float GetChargeUp()
    {
        return 0f;
    }

    /// <summary>
    /// Similar to CanAttack, individual Cooldowns willl be handled in main Attack() function.
    /// </summary>
    /// <returns></returns>
    public float GetCooldown()
    {
        return 0f;
    }

    /// <summary>
    /// Considered initialized as long as we have attackers wrapped.
    /// </summary>
    /// <returns></returns>
    public bool IsInitialized()
    {
        return attackers.Count > 0;
    }

    public IEnumerable<IAttackNode> GetChildren()
    {
        foreach (var attacker in attackers)
        {
            if (attacker is IAttackNode node)
            {
                foreach (var child in node.GetChildren())
                    yield return child;
            }
            else
            {
                yield return attacker as IAttackNode;
            }
        }
    }
}
