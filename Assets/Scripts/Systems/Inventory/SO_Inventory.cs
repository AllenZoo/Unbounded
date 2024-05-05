using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class SO_Inventory : ScriptableObject
{
    public int slots = 9;
    public List<Item> items = new List<Item>();

    public event Action OnInventoryDataChange;

    private void OnValidate()
    {
        // Match size of items to slots
        if (items.Count > slots)
        {
            items.RemoveRange(slots, items.Count - slots);
        }
        else if (items.Count < slots)
        {
            int difference = slots - items.Count;
            for (int i = 0; i < difference; i++)
            {
                items.Add(null);
            }
        }
    }

    public void InvokeOnDataChange()
    {
        //EventBus<OnInventoryModifiedEvent>.Call(new OnInventoryModifiedEvent());
        OnInventoryDataChange?.Invoke();
    }

    /// <summary>
    /// Returns the number of items (non null) in the inventory.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        foreach (Item item in items)
        {
            if (!item.IsEmpty())
            {
                return false;
            }
        }
        return true;
    }

}
