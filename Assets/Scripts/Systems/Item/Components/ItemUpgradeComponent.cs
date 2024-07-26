using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemUpgradeComponent : IItemComponent
{
    public List<StatModifierEquipment> upgradeStatModifiers = new List<StatModifierEquipment>();

    public ItemUpgradeComponent()
    {
        upgradeStatModifiers = new List<StatModifierEquipment>();
    }

    public ItemUpgradeComponent(List<StatModifierEquipment> toBeCloned)
    {
        this.upgradeStatModifiers = new List<StatModifierEquipment>();
        foreach (var statModifier in toBeCloned)
        {
            this.upgradeStatModifiers.Add(statModifier.DeepCopy());
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        // Check if lists match each other regardless of order.
        ItemUpgradeComponent other = obj as ItemUpgradeComponent;
        bool isEqual = upgradeStatModifiers.All(other.upgradeStatModifiers.Contains) && upgradeStatModifiers.Count == other.upgradeStatModifiers.Count;
        return isEqual;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(upgradeStatModifiers.GetHashCode());
    }
}
