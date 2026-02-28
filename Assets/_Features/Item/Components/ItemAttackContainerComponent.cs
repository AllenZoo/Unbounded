using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * NOTE: This class is not really used anymore, except for in the ItemDataConverter, where we check if an item has this component to determine if we can view UPGRADES for the item.
 *         -tldr this class acts as a tag for items that are weapons.
 *         
 *      Note that ItemData also has an IAttacker field.
 */
[Serializable]
public class ItemAttackContainerComponent : IItemComponent
{
    // TODO: privated since we won't be using this other than for verifying item conditions.? Probably remove this class entirely at some point since
    //       Items will now always have an Attacker component but set it to null if it can't attack. or doesn't do anything.
    private IAttacker attackerData;

    public ItemAttackContainerComponent(IAttacker attackerData)
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
        unchecked
        {
            return attackerData != null ? attackerData.GetHashCode() : 0;
        }
    }


    //public override string ToString()
    //{
    //    return string.Format("[Item Attack Component: {0}]", attackerData);
    //}
    #endregion
}

