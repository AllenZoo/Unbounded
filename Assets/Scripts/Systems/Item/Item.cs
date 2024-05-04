using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IItemComponentContainer
{
    public abstract List<IItemComponent> GetItemComponents();
}

public interface IItemComponent
{

}

[System.Serializable]
public class Item
{
    [SerializeField] public SO_Item data;
    [SerializeField] public int quantity;

    public Item(SO_Item data, int quantity)
    {
        this.data = data;
        this.quantity = quantity;
    }

    /// <summary>
    /// Check if the item is empty. If data is null or quantity is 0, the item is empty.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return data == null || quantity == 0;
    }

    /// <summary>
    /// Override Equals method.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Item other = obj as Item;
        return data == other.data && quantity == other.quantity;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(data.GetHashCode(), quantity);
    }
}
