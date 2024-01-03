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
}
