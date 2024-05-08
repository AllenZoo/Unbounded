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
public abstract class Item
{
    // SO_Item is persistent/init data.
    // TODO: change logic here.
    public SO_Item data;
    public int quantity;
    public IEquipItemBehaviour equipBehaviour;
    public IUpgradeItemBehaviour upgradeBehaviour;
    public IAttackItemBehaviour attackBehaviour;

    /// <summary>
    /// Creates a deep copy of the item.
    /// </summary>
    /// <returns></returns>
    public abstract Item Clone();
    #region Utilities
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
    #endregion
}

/// <summary>
/// C# class that represents state of basic item data.
/// </summary>
public class BaseItemComponent
{
    public string itemName;
    public Sprite itemSprite;
    public float spriteRot;
    public bool isStackable;
    public string description;
}

