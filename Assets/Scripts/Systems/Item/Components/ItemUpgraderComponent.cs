using System.Collections.Generic;
using UnityEngine;

/**
 * For items that upgrade other items.
 */
public class ItemUpgraderComponent : IItemComponent
{
    [Tooltip("The list of modifiers that will be applied to the item being upgraded.")]
    public List<StatModifierEquipment> modifiers = new List<StatModifierEquipment>();

    [Tooltip("The cost per item to upgrade. Eg. # of upgrade attack stones * costPerItem = final cost.")]
    public float costPerItem;

    public ItemUpgraderComponent()
    {

    }

    public ItemUpgraderComponent(List<StatModifierEquipment> modifiers, float costPerItem)
    {
        this.modifiers = modifiers;
        this.costPerItem = costPerItem;
    }
}
