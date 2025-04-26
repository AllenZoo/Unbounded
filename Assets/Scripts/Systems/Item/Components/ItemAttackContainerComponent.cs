using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * TODO: Deprecate, since item data contains attack data component too.
 * 
 * Actually: maybe don't deprecate since we might need to dynamically modify attacker traits (maybe through this class)
 * eg. add modifiers that add piercing, or smt.
 */
[Serializable]
public class ItemAttackContainerComponent : IItemComponent
{
    // TODO: privated since we won't be using this other than for verifying item conditions.? Probably remove this class entirely at some point since
    //       Items will now always have an Attacker component but set it to null if it can't attack. or doesn't do anything.
    private Attacker attackerData;

    public ItemAttackContainerComponent(Attacker attackerData)
    {
         this.attackerData = attackerData;
    }

    public IItemComponent DeepClone()
    {
        return new ItemAttackContainerComponent(attackerData);
    }

    #region Equals + Hash + ToString
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        ItemAttackContainerComponent other = obj as ItemAttackContainerComponent;

        if (attackerData == null)
        {
            return other.attackerData == null;
        }

        
        return attackerData.Equals(other.attackerData);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(attackerData.GetHashCode());
    }

    //public override string ToString()
    //{
    //    return string.Format("[Item Attack Component: {0}]", attackerData);
    //}
    #endregion
}

