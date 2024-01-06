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

    public void AddItem(int index, SO_Item item)
    {
        data.items[index] = item;
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

    // Stack items in the same inventory system
    // Move item from index1 into index2
    // Assumes index1 and index2 are stackable. TODO: add check here in case.
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
