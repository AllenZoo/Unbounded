using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages run history tracking and persistence.
/// Tracks weapons used during a run and saves completed run data.
/// Implements IDataPersistence to save/load high score and run history.
/// </summary>
public class RunHistoryManager : MonoBehaviour, IDataPersistence
{
    private const int MAX_RUN_HISTORY = 50;

    [SerializeField] private int currentHighScore = 0;
    [SerializeField] private List<RunHistoryData> runHistory = new List<RunHistoryData>();
    
    // Track weapons used during current run
    private List<WeaponUsageData> currentRunWeapons = new List<WeaponUsageData>();
    private HashSet<string> trackedWeaponIds = new HashSet<string>();
    private float runStartTime;

    private EventBinding<OnGameOverEvent> gameOverBinding;
    private EventBinding<OnInventoryModifiedEvent> inventoryModifiedBinding;
    
    // Cached references for performance
    private GameObject cachedPlayer;
    private InventorySystem cachedInventorySystem;
    
    // Static instance for easy access
    public static RunHistoryManager Instance { get; private set; }

    public int HighScore => currentHighScore;
    public List<RunHistoryData> RunHistory => new List<RunHistoryData>(runHistory);

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        gameOverBinding = new EventBinding<OnGameOverEvent>(HandleGameOver);
        EventBus<OnGameOverEvent>.Register(gameOverBinding);

        inventoryModifiedBinding = new EventBinding<OnInventoryModifiedEvent>(HandleInventoryModified);
        EventBus<OnInventoryModifiedEvent>.Register(inventoryModifiedBinding);
    }

    private void OnDisable()
    {
        if (gameOverBinding != null)
            EventBus<OnGameOverEvent>.Unregister(gameOverBinding);
        
        if (inventoryModifiedBinding != null)
            EventBus<OnInventoryModifiedEvent>.Unregister(inventoryModifiedBinding);
    }

    /// <summary>
    /// Called at the start of a new run to reset tracking.
    /// </summary>
    public void StartNewRun()
    {
        currentRunWeapons.Clear();
        trackedWeaponIds.Clear();
        runStartTime = Time.time;
        
        // Cache references at the start of a new run
        cachedPlayer = null;
        cachedInventorySystem = null;
        
        Debug.Log("[RunHistoryManager] Started tracking new run");
    }

    /// <summary>
    /// Handles inventory modifications to track weapon equips.
    /// </summary>
    private void HandleInventoryModified(OnInventoryModifiedEvent evt)
    {
        // Check if a weapon is equipped (slot 0 in equipment inventory)
        // We need to find the player's inventory to check this
        TrackCurrentWeapon();
    }

    /// <summary>
    /// Tracks the currently equipped weapon if not already tracked.
    /// </summary>
    private void TrackCurrentWeapon()
    {
        try
        {
            // Cache player reference if needed
            if (cachedPlayer == null)
            {
                cachedPlayer = GameObject.FindGameObjectWithTag("Player");
            }
            if (cachedPlayer == null) return;

            // Cache inventory system reference if needed
            if (cachedInventorySystem == null)
            {
                cachedInventorySystem = FindObjectOfType<InventorySystem>();
            }
            if (cachedInventorySystem == null) return;

            Item currentWeapon = cachedInventorySystem.GetItem(0); // Weapon slot is index 0
            if (currentWeapon == null || currentWeapon.IsEmpty()) return;
            
            string weaponId = currentWeapon.Data.ID;
            
            // Only track each unique weapon once per run
            if (!trackedWeaponIds.Contains(weaponId))
            {
                trackedWeaponIds.Add(weaponId);
                WeaponUsageData weaponData = new WeaponUsageData(
                    weaponId,
                    currentWeapon.Data.itemName,
                    Time.time - runStartTime
                );
                currentRunWeapons.Add(weaponData);
                Debug.Log($"[RunHistoryManager] Tracked weapon: {currentWeapon.Data.itemName}");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[RunHistoryManager] Failed to track weapon: {e.Message}");
        }
    }

    /// <summary>
    /// Handles game over event to save run history.
    /// </summary>
    private void HandleGameOver(OnGameOverEvent evt)
    {
        if (evt.scoreSummary == null)
        {
            Debug.LogWarning("[RunHistoryManager] Received null score summary");
            return;
        }

        // Make sure we track the final weapon
        TrackCurrentWeapon();

        // Create run history record
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        RunHistoryData runData = new RunHistoryData(evt.scoreSummary, currentRunWeapons, timestamp);

        // Update high score if needed
        if (runData.score > currentHighScore)
        {
            currentHighScore = runData.score;
            Debug.Log($"[RunHistoryManager] New high score: {currentHighScore}");
        }

        // Add to history with circular buffer logic
        runHistory.Add(runData);
        if (runHistory.Count > MAX_RUN_HISTORY)
        {
            runHistory.RemoveAt(0); // Remove oldest
        }

        Debug.Log($"[RunHistoryManager] Saved run: Score={runData.score}, Weapons={runData.weaponsUsed.Count}, History size={runHistory.Count}");

        // Save data immediately
        SaveData();
    }

    /// <summary>
    /// Saves data through the DataPersistenceHandler.
    /// </summary>
    private void SaveData()
    {
        DataPersistenceHandler handler = DataPersistenceHandler.Instance;
        if (handler != null)
        {
            handler.SaveGame();
        }
    }

    #region IDataPersistence Implementation

    public void LoadData(GameData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[RunHistoryManager] Cannot load from null GameData");
            return;
        }

        currentHighScore = data.highScore;
        runHistory = data.runHistory ?? new List<RunHistoryData>();
        
        Debug.Log($"[RunHistoryManager] Loaded: High Score={currentHighScore}, History size={runHistory.Count}");
    }

    public void SaveData(GameData data)
    {
        if (data == null)
        {
            Debug.LogWarning("[RunHistoryManager] Cannot save to null GameData");
            return;
        }

        data.highScore = currentHighScore;
        data.runHistory = runHistory;
        
        Debug.Log($"[RunHistoryManager] Saved: High Score={currentHighScore}, History size={runHistory.Count}");
    }

    #endregion
}
