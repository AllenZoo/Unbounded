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
    public static SO_Item CreateInstance(string itemName, Sprite itemSprite, float spriteRot, bool isStackable, string description)
    {
        var data = ScriptableObject.CreateInstance<SO_Item>();
        data.Init(itemName, itemSprite, spriteRot, isStackable, description);
        return data;
    }
    public static SO_Item CreateInstance(SO_Item data)
    {
        return CreateInstance(data.itemName, data.itemSprite, data.spriteRot, data.isStackable, data.description);
    }
    public void Init(string itemName, Sprite itemSprite, float spriteRot, bool isStackable, string description)
    {
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.spriteRot = spriteRot;
        this.isStackable = isStackable;
        this.description = description;
    }
    public abstract SO_Item Clone();
    public override int GetHashCode()
    {
        return HashCode.Combine(itemName.GetHashCode(), 
            isStackable.GetHashCode(), 
            description.GetHashCode());
    }
}
