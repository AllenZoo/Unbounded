using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Handles logic of managing data of inventory.
public class Inventory
{
    public event Action OnInventoryDataModified;
    private List<SO_Item> items;
    private int numSlots;

    // private SO_Inventory data;

    // Init through scriptable object.
    public Inventory(SO_Inventory inventory)
    {
        items = inventory.items;
        numSlots = inventory.slots;

        // Note: data.OnInventoryDataChange += OnInventoryDataModified does NOT work.
        inventory.OnInventoryDataChange += InvokeOnInventoryDataModified;
    }

    // Init through other params
    public Inventory(List<SO_Item> items, int numSlots)
    {
        // Assert that items.Count <= numSlots.
        Assert.IsTrue(items.Count <= numSlots, "Inventory items.Count must be less than or equal to numSlots.");

        this.items = items;
        this.numSlots = numSlots;

        // Check if items.Count is less than numSlots. If so add null until items.Count == numSlots.
        if (items.Count < numSlots)
        {
            int difference = numSlots - items.Count;
            for (int i = 0; i < difference; i++)
            {
                items.Add(null);
            }
        }
    }

    // When SO_Inventory invokes OnInventoryDataChange, invoke Inventory OnInventoryDataModified.
    // Modifications to data.items should only invoke data.OnInventoryChange.
    public void InvokeOnInventoryDataModified()
    {
        OnInventoryDataModified?.Invoke();
    }

    public void AddItem(int index, Item item)
    {
        items[index] = item.data;
        OnInventoryDataModified?.Invoke();
    }

    public void AddItem(int index, SO_Item item)
    {
        items[index] = item;
        OnInventoryDataModified?.Invoke();
    }

    public void RemoveItem(int index)
    {
        items[index] = null;
        OnInventoryDataModified?.Invoke();
    }

    public SO_Item GetItem(int index)
    {
        return items[index];
    }

    public void SwapItems(int index1, int index2)
    {
        SO_Item temp = items[index1];
        items[index1] = items[index2];
        items[index2] = temp;
        OnInventoryDataModified?.Invoke();
    }

    public int GetFirstEmptySlot()
    {
        for (int i = 0; i < numSlots; i++)
        {
            if (items[i] == null)
            {
                return i;
            }
        }
        return -1;
    }
}
