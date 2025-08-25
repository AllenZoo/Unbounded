using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that encapsulates all the data we want to save in the game.
/// </summary>
[Serializable]
public class GameData
{
    public long lastUpdated;
    public Item playerEquippedWeapon;

    /// <summary>
    /// Maps InventorySystem GUID to Invenotry.
    /// </summary>
    public Dictionary<string, Inventory> inventories;

    public GameData()
    {
        playerEquippedWeapon = null;
        inventories = new Dictionary<string, Inventory> ();
    }
}
