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

    /// <summary>
    /// Audio Settings
    /// </summary>
    public float backgroundMusicVolume;
    public float soundEffectsVolume;

    /// <summary>
    /// High Score and Run History
    /// </summary>
    public int highScore;
    public List<RunHistoryData> runHistory;

    public GameData()
    {
        playerEquippedWeapon = null;
        inventories = new Dictionary<string, Inventory> ();
        backgroundMusicVolume = 100f;
        soundEffectsVolume = 100f;
        highScore = 0;
        runHistory = new List<RunHistoryData>();
    }
}
