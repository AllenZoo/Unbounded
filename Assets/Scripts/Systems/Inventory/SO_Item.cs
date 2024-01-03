using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class SO_Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public Vector2 spriteDims;
    public int quantity;
    public bool isStackable;
    public string description;

    [Tooltip("If item is weapon, attach the corresponding weapon_item data here.")]
    public SO_Weapon_Item weaponItem;
}
