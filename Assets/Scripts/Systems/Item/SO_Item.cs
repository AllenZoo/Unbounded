using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Item")]
public class SO_Item : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public float spriteRot;
    public bool isStackable;
    public string description;

    public override string ToString()
    {
        return string.Format("[Item Name: {0}]", itemName);
    }
}
