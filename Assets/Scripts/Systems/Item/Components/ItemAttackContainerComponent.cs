using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemAttackContainerComponent : IItemComponent
{
    // TODO: privated since we won't be using this other than for verifying item conditions.? Probably remove this class entirely at some point since
    //       Items will now always have an Attacker component but set it to null if it can't attack. or doesn't do anything.
    private Attacker attackerData;

    public ItemAttackContainerComponent(Attacker attackerData)
    {
        // this.attackerData = attackerData;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        ItemAttackContainerComponent other = obj as ItemAttackContainerComponent;
        return attackerData.Equals(other.attackerData);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(attackerData.GetHashCode());
    }

    public override string ToString()
    {
        return string.Format("[Item Attack Component: {0}]", attackerData);
    }
}

