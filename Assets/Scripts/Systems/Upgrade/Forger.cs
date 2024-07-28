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

    /// <summary>
    /// 1. Clone the equipment.
    /// 2. Modify the clone with the upgrade components of the stones.
    /// 3. return it.
    /// </summary>
    /// <param name="stones"></param>
    /// <param name="equipment"></param>
    /// <returns>the preview item or null if invalid</returns>
    public Item Forge(List<Item> stones, Item equipment)
    {
        Item previewItem = equipment.Clone();

        if (!previewItem.HasComponent<ItemUpgradeComponent>())
        {
            Debug.LogError("Equipment does not have an upgrade component!");
            return null;
        }

        foreach (Item stone in stones)
        {
            if (stone == null || stone.IsEmpty()) continue;

            if (!stone.HasComponent<ItemUpgraderComponent>())
            {
                Debug.LogError("Stone does not have an upgrade component! This should never happen if the inventory slot conditions are correctly set!");
                return null;
            }

            for (int i = 0; i < stone.quantity; i++)
            {
                // Add stat modifiers to the previewItem
                previewItem.GetComponent<ItemUpgradeComponent>().upgradeStatModifiers.AddRange(stone.GetComponent<ItemUpgraderComponent>().modifiers);

                // Add the stones themselves into the preview item for history purposes
                previewItem.GetComponent<ItemUpgradeComponent>().upgrades.Add(stone);
            }
        }

        return previewItem;
    }
}
