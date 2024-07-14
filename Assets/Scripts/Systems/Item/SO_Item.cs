using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Item")]
public abstract class SO_Item : ScriptableObject, IItemComponentContainer
{
    public string itemName;
    public Sprite itemSprite;
    public float spriteRot;
    public bool isStackable;
    public string description;

    //public override bool Equals(object other)
    //{
    //    if (other == null || GetType() != other.GetType())
    //    {
    //        return false;
    //    }

    //    SO_Item otherObj = other as SO_Item;
    //    return itemName == otherObj.itemName && 
    //        isStackable == otherObj.isStackable &&
    //        description == otherObj.description;
    //}

    //public override int GetHashCode()
    //{
    //    return HashCode.Combine(itemName.GetHashCode(),
    //        isStackable.GetHashCode(),
    //        description.GetHashCode());
    //}

    public virtual List<IItemComponent> GetItemComponents()
    {
        return new List<IItemComponent>();
    }
    public void InitBase(string itemName, Sprite itemSprite, float spriteRot, bool isStackable, string description)
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.spriteRot = spriteRot;
        this.isStackable = isStackable;
        this.description = description;
    }
    public abstract SO_Item Clone();
    public abstract override bool Equals(object other);
    public abstract override int GetHashCode();
    

    public override string ToString()
    {
        return string.Format("[Item Name: {0}]", itemName);
    }
    
}
