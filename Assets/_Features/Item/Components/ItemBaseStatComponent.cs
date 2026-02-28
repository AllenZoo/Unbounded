using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    [field: SerializeField] public SO_StatContainer BaseStats { get; private set; }
    [OdinSerialize, ShowInInspector, ReadOnly] public string BaseStatGUID;

    #region Constructors
    public ItemBaseStatComponent()
    {

    }

    public ItemBaseStatComponent(SO_StatContainer baseStats)
    {
        this.BaseStats = baseStats;

        if (baseStats != null)
        {
            BaseStatGUID = baseStats.ID;
        }
    }

    public virtual void Init()
    {
        if (BaseStats != null)
        {
            BaseStatGUID = BaseStats.ID;
        }
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

    #region Data Persistence

    public virtual void Load(Item item)
    {
        var bsComp = item.GetComponent<ItemBaseStatComponent>();
        if (bsComp != null)
        {
            // Load the ItemData from Database
            BaseStats = ScriptableObjectDatabase.Instance.Data.Get<SO_StatContainer>(bsComp.BaseStatGUID);
        }
    }

    #endregion
}
