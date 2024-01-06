using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Handles logic of managing data of inventory.
public class Inventory
{
    public event Action OnInventoryDataModified;
    private SO_Inventory data;

    //private List<SO_Item> items;
    //private int numSlots;

    // Init through scriptable object.
    public Inventory(SO_Inventory inventory)
    {
        // Passes by ref!
        //items = inventory.items;
        //numSlots = inventory.slots;
        data = inventory;

        // Note: data.OnInventoryDataChange += OnInventoryDataModified does NOT work.
        inventory.OnInventoryDataChange += InvokeOnInventoryDataModified;
    }

    // Init through other params
    //public Inventory(List<SO_Item> items, int numSlots)
    //{
    //    // Assert that items.Count <= numSlots.
    //    Assert.IsTrue(items.Count <= numSlots, "Inventory items.Count must be less than or equal to numSlots.");

    //    this.items = items;
    //    this.numSlots = numSlots;

    //    // Check if items.Count is less than numSlots. If so add null until items.Count == numSlots.
    //    if (items.Count < numSlots)
    //    {
    //        int difference = numSlots - items.Count;
    //        for (int i = 0; i < difference; i++)
    //        {
    //            items.Add(null);
    //        }
    //    }
    //}

    // When SO_Inventory invokes OnInventoryDataChange, invoke Inventory OnInventoryDataModified.
    // Modifications to data.items should only invoke data.OnInventoryChange.
    public void InvokeOnInventoryDataModified()
    {
        OnInventoryDataModified?.Invoke();
    }

    public void AddItem(int index, Item item)
    {
        data.items[index] = item.data;
        data.InvokeOnDataChange();
        // OnInventoryDataModified?.Invoke();
    }

    // Attempts to add/stack an item to an index of the inventory. 
    public void AddItem(int index, SO_Item item)
    {
        SO_Item item1 = data.items[index];
        SO_Item item2 = item;

        if (item1 == null)
        {
            data.items[index] = item;
        } else if (item1.itemName == item2.itemName
            && item1.isStackable && item2.isStackable)
        {
            // Stack items.

            // Create new reference to item in inventory.
            SO_Item copy = data.items[index].Copy();
            copy.quantity += item.quantity;

            data.items[index] = copy;
        } else
        {
            Debug.LogError("Cannot add item to inventory. " +
                "Item at index is not null and does not match item to add." +
                "Failed to guard somewhere.");
        }
        
        data.InvokeOnDataChange();
        // OnInventoryDataModified?.Invoke();
    }

    public void RemoveItem(int index)
    {
        data.items[index] = null;
        data.InvokeOnDataChange();
        // OnInventoryDataModified?.Invoke();
    }

    public SO_Item GetItem(int index)
    {
        return data.items[index];
    }

    public void SwapItems(int index1, int index2)
    {
        SO_Item temp = data.items[index1];
        data.items[index1] = data.items[index2];
        data.items[index2] = temp;
        data.InvokeOnDataChange();
        // OnInventoryDataModified?.Invoke();
    }

    /// <summary>
    /// Stack items in the same inventory system. 
    /// Move item from index1 into index2
    /// Assumes index1 and index2 are stackable. TODO: add check here in case.
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    public void StackItems(int index1, int index2)
    {
        // Create a new object (so that we don't modify the original object by ref).
        int newStackCount = data.items[index1].quantity + data.items[index2].quantity;
        SO_Item newItem = data.items[index1].Copy();
        newItem.quantity = newStackCount;

        data.items[index2] = newItem;
        data.items[index1] = null;
        data.InvokeOnDataChange();
    }

    /// <summary>
    /// Splits the item at index in half. If item quantity is odd, 
    /// the remainder is placed in the first half (not the returned SO_Item).
    /// Creates two new SO_Items.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The other half of the item</returns>
    public SO_Item SplitIndex(int index)
    {
        SO_Item firstHalf = data.items[index].Copy();
        int totalQuantity = firstHalf.quantity;

        if (totalQuantity <= 1)
        {
            // Not splittable, return error!
            Debug.LogError("Should not have requested to split an item with quantity <= 0." +
                "Missing guard!");
            return null;
        }

        firstHalf.quantity = Mathf.CeilToInt(totalQuantity / 2f);
        data.items[index] = firstHalf;

        SO_Item secondHalf = data.items[index].Copy();
        secondHalf.quantity = Mathf.FloorToInt(totalQuantity / 2f);

        data.InvokeOnDataChange();

        return secondHalf;
    }

    public int GetFirstEmptySlot()
    {
        for (int i = 0; i < data.slots; i++)
        {
            if (data.items[i] == null)
            {
                return i;
            }
        }
        return -1;
    }
}
