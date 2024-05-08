using System;
using System.Collections;
using System.Collections.Generic;
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
}
