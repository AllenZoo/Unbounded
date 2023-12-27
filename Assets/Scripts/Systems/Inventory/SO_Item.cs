using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class SO_Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public int quantity;
    public bool isStackable;
}
