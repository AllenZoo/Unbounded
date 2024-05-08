using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IForgeable
{
    // public void Forge();
}

// Data containing weapon_item data of a weapon item.
// Use in inheritance with SO_Item.
[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Item/Weapon Item")]
public class SO_Weapon_Item : SO_Item, IItemComponentContainer, IForgeable
{
    public ItemAttackComponent itemAttackComponent;

    [Tooltip("Stats from base item")]
    public ItemStatComponent itemStatComponent;

    [Tooltip("Stats from upgrades")]
    public ItemUpgradeComponent itemUpgradeComponent;

    public override SO_Item Clone()
    {
        var data = ScriptableObject.CreateInstance<SO_Weapon_Item>();
        data.Init(itemName, itemSprite, spriteRot, isStackable, description);

        data.itemAttackComponent = new ItemAttackComponent(itemAttackComponent.attackerData);
        data.itemStatComponent = new ItemStatComponent(itemStatComponent.statModifiers);
        data.itemUpgradeComponent = new ItemUpgradeComponent(itemUpgradeComponent.upgradeStatModifiers);
        return data;
    }

    public override List<IItemComponent> GetItemComponents()
    {
        return new List<IItemComponent>() { itemAttackComponent, itemStatComponent, itemUpgradeComponent};
    }
}
