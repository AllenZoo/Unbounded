using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles logic of managing data of inventory.
public class Inventory
{
    public event Action OnInventoryDataModified;
    private List<SO_Item> items;

    private SO_Inventory data;

    // Init through scriptable object.
    public Inventory(SO_Inventory inventory)
    {
        this.data = inventory;
        items = inventory.items;

        // Note: data.OnInventoryDataChange += OnInventoryDataModified does NOT work.
        data.OnInventoryDataChange += InvokeOnInventoryDataModified;
    }

    public Inventory(List<SO_Item> items)
    {
        this.items = items;
    }

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
    }

    public void AddItem(int index, SO_Item item)
    {
        data.items[index] = item;
        data.InvokeOnDataChange();
    }

    public void RemoveItem(int index)
    {
        data.items[index] = null;
        data.InvokeOnDataChange();
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
