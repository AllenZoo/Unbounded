using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * For items that have base stats.
 */
[Serializable]
public class ItemBaseStatComponent : IItemComponent
{
    /// <summary>
    /// Base stats of the weapon. Static.
    /// </summary>
    // [field: SerializeField]
    public SO_StatContainer BaseStats { get; private set; }

    ///// <summary>
    ///// The Stat modifiers applied to item.
    ///// TODO: move all modifier logic to upgrade component.
    ///// </summary>
    //public List<StatModifierEquipment> statModifiers = new List<StatModifierEquipment>();

    #region Constructors
    public ItemBaseStatComponent()
    {
    }

    public ItemBaseStatComponent(SO_StatContainer baseStats)
    {
        this.BaseStats = baseStats;
    }
    #endregion

    public IItemComponent DeepClone()
    {
        return new ItemBaseStatComponent(BaseStats);
    }

    #region Equals + Hash
    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        ItemBaseStatComponent other = obj as ItemBaseStatComponent;
        if (BaseStats == null)
        {
            return other.BaseStats == null;
        }

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        bool isEqual = BaseStats.Equals(other.BaseStats);
        return isEqual;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BaseStats.GetHashCode());
    }
    #endregion
}
