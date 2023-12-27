using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles logic of managing data of inventory.
public class Inventory
{
    private SO_Inventory data;

    public Inventory(SO_Inventory inventory)
    {
        this.data = inventory;
    }

    public void AddItem(int index, Item item)
    {
        data.items[index] = item.data;
    }

    public void RemoveItem(int index)
    {
        data.items[index] = null;
    }

    public void SwapItems(int index1, int index2)
    {
        SO_Item temp = data.items[index1];
        data.items[index1] = data.items[index2];
        data.items[index2] = temp;
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
