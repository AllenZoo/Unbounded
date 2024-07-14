using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade Item", menuName = "Item/Upgrade Item")]
public class SO_UpgradeItem : SO_Item, IItemComponentContainer
{
    public ItemUpgradeComponent itemUpgradeComponent;
    public ItemValueComponent itemValueComponent;

    public override SO_Item Clone()
    {
        throw new System.NotImplementedException();
    }

    public override bool Equals(object other)
    {
        Debug.Log("Called (SO_UpgradeItem) Equals");
        
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }
        SO_UpgradeItem o = other as SO_UpgradeItem;
        return itemName == o.itemName &&
            itemUpgradeComponent.Equals(o.itemUpgradeComponent) &&
            itemValueComponent.Equals(o.itemValueComponent);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(itemName.GetHashCode(),
                       itemUpgradeComponent.GetHashCode(),
                                  itemValueComponent.GetHashCode());
    }

    public override List<IItemComponent> GetItemComponents()
    {
        return new List<IItemComponent>() { itemUpgradeComponent, itemValueComponent };
    }
}

