using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemAttackContainerComponent : IItemComponent
{
    public Attacker attackerData;

    public ItemAttackContainerComponent(Attacker attackerData)
    {
        this.attackerData = attackerData;
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

