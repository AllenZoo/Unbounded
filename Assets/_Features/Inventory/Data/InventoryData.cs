using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;

/// <summary>
/// Class is deprecated since we no longer want to use SO to store dynamic data.
/// </summary>
//[CreateAssetMenu(fileName = "New Inventory", menuName = "System/Inventory/InventoryData")]
public class InventoryData : SerializedScriptableObject
{
    public List<ItemData> items;
}