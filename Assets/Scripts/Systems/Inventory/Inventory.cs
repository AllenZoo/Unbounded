using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Assertions;

// Handles logic of managing data of inventory.
[System.Serializable]
public class Inventory
{
    public event Action OnInventoryDataModified;

    [SerializeField] private List<Item> items = new List<Item>();
    [SerializeField] private int numSlots = 9;

    // Init through scriptable object.
    public Inventory()
    {
        items = new List<Item>();
    }

    public Inventory(List<Item> items, int numSlots)
    {
        // Assert that items.Count <= numSlots.
        Assert.IsTrue(items.Count <= numSlots, "Inventory items.Count must be less than or equal to numSlots.");

        this.items = items;
        this.numSlots = numSlots;

        // Check if items.Count is less than numSlots. If so add null until items.Count == numSlots.
        if (this.items.Count < this.numSlots)
        {
            int difference = this.numSlots - this.items.Count;
            for (int i = 0; i < difference; i++)
            {
                this.items.Add(null);
            }
        }
    }

    public void Init()
    {
        AdjustItemsToSlots();
        // Necessary so that the ItemModifierMediator in Item gets initialized properly since not serializable.
        foreach (Item item in items)
        {
            if (item != null) item.Init();
        }
    }

   
    /// <summary>
    /// Attempts to add/stack an item to an index of the inventory. 
    /// </summary>
    /// <param name="index">index to add item to.</param>
    /// <param name="item">item to add/stack.</param>
    /// <returns>1 if item was added/stacked, -1 if item was not added/stacked.</returns>
    public int AddItem(int index, Item item)
    {
        Item item1 = items[index];
        Item item2 = item;

        if (item1 == null || item1.data == null)
        {
            items[index] = item;
        }
        else if (item1.data.Equals(item2.data)
            && item1.data.isStackable && item2.data.isStackable)
        {
            // Stack items.
            Item stackedItem = item1.Clone();
            stackedItem.quantity += item2.quantity;
            items[index] = stackedItem;
        }
        else
        {
            Debug.Log("Cannot add/stack item to inventory. " +
                "Item at index is not null and does not match item to add.");
            return -1;
        }

        EventBus<OnInventoryModifiedEvent>.Call(new OnInventoryModifiedEvent());
        OnInventoryDataModified?.Invoke();
        return 1;
    }

    public void RemoveItem(int index)
    {
        items[index] = null;
        EventBus<OnInventoryModifiedEvent>.Call(new OnInventoryModifiedEvent());
        OnInventoryDataModified?.Invoke();
    }

    public void SetItem(int index, Item item)
    {
        items[index] = item;
        EventBus<OnInventoryModifiedEvent>.Call(new OnInventoryModifiedEvent());
        OnInventoryDataModified?.Invoke();
    }

    public Item GetItem(int index)
    {
        return items[index];
    }

    public void SwapItems(int index1, int index2)
    {
        Item temp = items[index1];
        items[index1] = items[index2];
        items[index2] = temp;
        EventBus<OnInventoryModifiedEvent>.Call(new OnInventoryModifiedEvent());
        OnInventoryDataModified?.Invoke();
    }

    /// <summary>
    /// Checks if index1 and index2 are stackable. If they are, stack them.
    /// Stack items in the same inventory system. 
    /// Move item from index1 into index2
    /// 
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    /// <returns>1 if successful, -1 if failed</returns>
    public int StackItems(int index1, int index2)
    {
        Item item1 = items[index1];
        Item item2 = items[index2];

        // Check if items are stackable and have the same SO_Item data.
        if (item2 != null && item2.data != null && 
                       (( !item1.data.Equals(item2.data)
                       || !item1.data.isStackable
                       || !item2.data.isStackable) ))
        {
            //Debug.Log("Cannot stack items. " +
            //    "Items are not stackable or do not have the same SO_Item data.");
            return -1;
        }

        AddItem(index2, items[index1]);
        return 1;
    }

    /// <summary>
    /// Splits the item at index in half. If item quantity is odd, 
    /// the remainder is placed in the first half (not the returned Item).
    /// Creates two new Item.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The second half of the items. Returns null if item is not splittable.</returns>
    public Item SplitIndex(int index)
    {
        Item originalItem = items[index];
        int totalQuantity = originalItem.quantity;
        // To be splittable, the item must be stackable and have a quantity > 1.
        if (totalQuantity <= 1 && originalItem.data.isStackable)
        {
            // Not splittable, return error!
            Debug.Log("Attempted to split item. Failed.");
            return null;
        }

        int firstHalfQuantity = Mathf.CeilToInt(totalQuantity / 2f);
        int secondHalfQuantity = Mathf.FloorToInt(totalQuantity / 2f);

        originalItem.quantity = firstHalfQuantity;

        EventBus<OnInventoryModifiedEvent>.Call(new OnInventoryModifiedEvent());
        OnInventoryDataModified?.Invoke();

        Item secondHalf = originalItem.Clone();
        secondHalf.quantity = secondHalfQuantity;
        return secondHalf;
    }

    /// <summary>
    /// Returns the index of the last instance of the item in the inventory.
    /// Returns -1 if the item is not in the inventory.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>index or -1</returns>
    public int LastIndexOf(Item item)
    {
        for (int i = numSlots - 1; i >= 0; i--)
        {
            if (items[i] == item)
            {
                return i;
            }
        }
        return -1;
    }

    public int GetFirstEmptySlot()
    {
        for (int i = 0; i < numSlots; i++)
        {

            // TODO: check logic. (Do we need to add .data or just items[i] == null?)
            if (items[i].data == null)
            {
                return i;
            }
        }
        return -1;
    }

    public bool IsEmpty()
    {
        for(int i = 0;i < numSlots;i++)
        {
            if (items[i] != null && items[i].data != null)
            {
                return false;
            }
        }
        return true;
    }

    public int GetInventorySize()
    {
        return numSlots;
    }
    public void ClearInventory()
    {
        items.Clear();
        AdjustItemsToSlots();
    }
    private void AdjustItemsToSlots()
    {
        if (items.Count > numSlots)
        {
            items.RemoveRange(numSlots, items.Count - numSlots);
        }
        else if (items.Count < numSlots)
        {
            int difference = numSlots - items.Count;
            for (int i = 0; i < difference; i++)
            {
                items.Add(null);
            }
        }
        OnInventoryDataModified?.Invoke();
    }
}
