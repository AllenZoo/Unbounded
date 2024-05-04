using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data containing weapon_item data of a weapon item.
// Use in inheritance with SO_Item.
[CreateAssetMenu(fileName = "New Weapon Item", menuName = "Item/Weapon Item")]
public class SO_Weapon_Item : SO_Item, IItemComponentContainer
{
    public ItemAttackComponent itemAttackComponent;
    public ItemStatComponent itemStatComponent;

    public override List<IItemComponent> GetItemComponents()
    {
        return new List<IItemComponent>() { itemAttackComponent, itemStatComponent};
    }
}
