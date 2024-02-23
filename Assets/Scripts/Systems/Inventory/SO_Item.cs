using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class SO_Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public float spriteRot;
    public bool isStackable;
    public string description;

    public List<IStatModifier> statModifiers = new List<IStatModifier>();

    //[Tooltip("If item is weapon, attach the corresponding weapon_item data here.")]
    //public SO_Weapon_Item weaponItem;

    [Tooltip("If item is weapon, attach the corresponding attacker data here.")]
    public SO_Attacker attacker;

    public override bool Equals(object other)
    {
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }

        SO_Item otherObj = other as SO_Item;
        return itemName == otherObj.itemName && 
            itemSprite == otherObj.itemSprite &&
            spriteRot == otherObj.spriteRot &&
            isStackable == otherObj.isStackable &&
            description == otherObj.description;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(itemName.GetHashCode(), 
            itemSprite.GetHashCode(), 
            spriteRot.GetHashCode(), 
            isStackable.GetHashCode(), 
            description.GetHashCode());
    }
}
