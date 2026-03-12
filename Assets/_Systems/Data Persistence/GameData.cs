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
    public string gameVersion;
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

    public SceneField currentScene;
    public float playerCurrentHealth;
    public float playerGold;

    /// <summary>
    /// Objectives
    /// </summary>
    public bool tutorialComplete;
    //public Dictionary<string, ObjectiveState> objectiveStates;

    /// <summary>
    /// Global Variables
    /// 
    /// Hold state for all ScriptableObjectBoolean objects.
    /// </summary>
    public Dictionary<string, bool> soBooleanStates;

    public GameData()
    {
        //gameVersion = Application.version;
        playerEquippedWeapon = null;
        inventories = new Dictionary<string, Inventory> ();
        backgroundMusicVolume = 100f;
        soundEffectsVolume = 100f;
        highScore = 0;
        runHistory = new List<RunHistoryData>();
        currentScene = new SceneField("");
        playerCurrentHealth = 0;
        playerGold = 0;
        //objectiveStates = new Dictionary<string, ObjectiveState>();
        tutorialComplete = false;
        soBooleanStates = new Dictionary<string, bool>();
    }
}
