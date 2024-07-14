using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public interface IForger
{
    /// <summary>
    /// Forges upgrades on the given equipment using the given stones. Returns the upgraded equipment.
    /// </summary>
    /// <param name="stones"></param>
    /// <param name="equipment"></param>
    /// <returns></returns>
    public Item Forge(List<Item> stones, Item equipment);
}
public class Forger : IForger
{

    public Forger()
    {

    }

    public Item Forge(List<Item> stones, Item equipment)
    {
        // 1. Clone data SO.
        // 2. Modify the upgrade component of that cloned SO.
        // 3. Create a new item with that cloned SO and return it.

        if (equipment.data is SO_Weapon_Item)
        {
            SO_Weapon_Item newData = equipment.data.Clone() as SO_Weapon_Item;
            ItemUpgradeComponent equipUpgrade = newData.GetItemComponents().Find(x => x is ItemUpgradeComponent) as ItemUpgradeComponent;

            foreach (Item stone in stones)
            {
                if (stone == null || stone.IsEmpty())
                {
                    continue;
                }
                  
                IItemComponent stoneUpgrade = stone.data.GetItemComponents().Find(x => x is ItemUpgradeComponent);
                if (stoneUpgrade != null)
                {
                    ItemUpgradeComponent stoneUpgradeComponent = stoneUpgrade as ItemUpgradeComponent;

                    for (int i = 0; i < stone.quantity; i++)
                    {
                        equipUpgrade.upgradeStatModifiers.AddRange(stoneUpgradeComponent.upgradeStatModifiers);
                    }
                }
            }

            Item previewWeapon = new Item(newData, equipment.quantity);
            return previewWeapon;
        }

        // Stub
        Debug.LogError("Should not get to here!");
        return equipment;
    }

    public Item Forge(Item stone, Item equipment)
    {
        return null; // stub
    }
}
