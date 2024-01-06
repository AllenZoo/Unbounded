using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class SO_Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public float spriteRot;
    public int quantity;
    public bool isStackable;
    public string description;

    public List<IStatModifier> statModifiers = new List<IStatModifier>();

    [Tooltip("If item is weapon, attach the corresponding weapon_item data here.")]
    public SO_Weapon_Item weaponItem;


    // To use via code when we want to duplicate SO_Items
    public SO_Item Copy()
    {
        SO_Item copy = CreateInstance<SO_Item>();

        // Copy values
        copy.itemName = this.itemName;
        copy.itemSprite = this.itemSprite;
        copy.spriteRot = this.spriteRot;
        copy.quantity = this.quantity;
        copy.isStackable = this.isStackable;
        copy.description = this.description;

        // Create a new list and copy the elements
        copy.statModifiers = new List<IStatModifier>(this.statModifiers);

        // Assuming IStatModifier is a class or a struct, this should be sufficient.
        // If it's a more complex object or references, you might need a deep copy.

        copy.weaponItem = this.weaponItem;

        return copy;
    }

}
