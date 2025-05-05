using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * For items that upgrade other items.
 * 
 * TODO: Deprecate, since we plan on removing items that upgrade other items. (eg. upgrade stones)
 */
[Serializable]
public class ItemUpgraderComponent : IItemComponent
{
    [Tooltip("The list of modifiers that will be applied to the item being upgraded.")]
    public List<StatModifierEquipment> modifiers = new List<StatModifierEquipment>();

    [Tooltip("The cost per item to upgrade. Eg. # of upgrade attack stones * costPerItem = final cost.")]
    public float costPerItem = 1f;

    #region Constructors
    public ItemUpgraderComponent()
    {

    }

    public ItemUpgraderComponent(List<StatModifierEquipment> modifiers, float costPerItem)
    {
        this.modifiers = modifiers;
        this.costPerItem = costPerItem;
    }
    #endregion

    public IItemComponent DeepClone()
    {
        return new ItemUpgraderComponent(modifiers, costPerItem);
    }
}
