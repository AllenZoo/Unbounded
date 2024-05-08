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

public interface IUpgradeItemBehaviour
{
    /// <summary>
    /// Upgrades the ItemStatComponent of an item.
    /// Returns true if the item was successfully upgraded.
    /// Returns false if we tried to upgrade an item with an item that cannot upgrade.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    bool Upgrade(ItemStatComponent i);
}

/// <summary>
/// For upgrading items with standard upgrade behaviour. (eg. upgrade stones)
/// </summary>
public class StandardUpgradeItemBehaviour : IUpgradeItemBehaviour
{
    private ItemUpgradeComponent upgradeComponent;
    public StandardUpgradeItemBehaviour(ItemUpgradeComponent upgradeComponent)
    {
        this.upgradeComponent = upgradeComponent;
    }

    public bool Upgrade(ItemStatComponent i)
    {
        if (i == null)
        {
            return false;
        }
        foreach (var statModifier in upgradeComponent.upgradeStatModifiers)
        {
            i.statModifiers.Add(statModifier);
        }
        return true;
    }
}

/// <summary>
/// For items with no upgrading behaviour. (eg. weapons, armor)
/// </summary>
public class NoUpgradeItemBehaviour : IUpgradeItemBehaviour
{
    public bool Upgrade(ItemStatComponent i)
    {
        return false;
    }
}