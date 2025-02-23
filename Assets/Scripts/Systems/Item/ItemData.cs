using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "System/Item/Item")]
[Serializable]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite itemSprite;
    public float spriteRot;
    public bool isStackable;
    public string description;

    [Tooltip("For weapons that contain some attack behaviour data.")]
    public Attacker attacker;

    public override string ToString()
    {
        return string.Format("[Item Name: {0}]", itemName);
    }
}
