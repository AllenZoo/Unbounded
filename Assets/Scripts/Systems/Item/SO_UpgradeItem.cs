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

    public override List<IItemComponent> GetItemComponents()
    {
        return new List<IItemComponent>() { itemUpgradeComponent, itemValueComponent };
    }
}

