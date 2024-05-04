using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Item")]
public class SO_Item : ScriptableObject, IItemComponentContainer
{
    public string itemName;
    public Sprite itemSprite;
    public float spriteRot;
    public bool isStackable;
    public string description;

    public override bool Equals(object other)
    {
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }

        SO_Item otherObj = other as SO_Item;
        return itemName == otherObj.itemName && 
            isStackable == otherObj.isStackable &&
            description == otherObj.description;
    }

    public virtual List<IItemComponent> GetItemComponents()
    {
        return new List<IItemComponent>();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(itemName.GetHashCode(), 
            isStackable.GetHashCode(), 
            description.GetHashCode());
    }

    
}
