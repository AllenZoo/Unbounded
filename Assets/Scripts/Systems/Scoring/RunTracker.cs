using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Central singleton component that tracks run statistics including damage dealt and boss fight times.
/// Listens to game events to update tracking data.
/// Avoids per-frame allocations for performance.
/// </summary>
public class RunTracker : Singleton<RunTracker>
{
    private RunData currentRun;
    private bool isTrackingBossFight = false;
    private string currentBossName = string.Empty;

    // Store event bindings for proper cleanup
    private EventBinding<OnBossFightStartEvent> bossStartBinding;
    private EventBinding<OnBossFightEndEvent> bossEndBinding;

    public RunData CurrentRun => currentRun;

    protected override void Awake()
    {
        base.Awake();
        currentRun = new RunData();
        
        // Register for boss fight events and store bindings for cleanup
        bossStartBinding = new EventBinding<OnBossFightStartEvent>(OnBossFightStart);
        EventBus<OnBossFightStartEvent>.Register(bossStartBinding);

        bossEndBinding = new EventBinding<OnBossFightEndEvent>(OnBossFightEnd);
        EventBus<OnBossFightEndEvent>.Register(bossEndBinding);
    }

    private void Start()
    {
        // Try to find and track player damage events
        // Look for player entity in scene
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            LocalEventHandler playerLEH = player.GetComponent<LocalEventHandler>();
            if (playerLEH != null)
            {
                RegisterPlayerDamageTracking(playerLEH);
            }
            else
            {
                Debug.LogWarning("RunTracker: Player found but no LocalEventHandler component.");
            }
        }
    }

    /// <summary>
    /// Registers to track damage dealt by the player's attacks.
    /// This should be called when the player is spawned/initialized.
    /// </summary>
    public void RegisterPlayerDamageTracking(LocalEventHandler playerLEH)
    {
        if (playerLEH == null) return;

        // We need to listen for when player deals damage to enemies
        // This will be handled through attack hit events
        Debug.Log("RunTracker: Registered player damage tracking");
    }

    /// <summary>
    /// Manually record damage dealt by player to boss.
    /// Call this from attack systems when damage is confirmed.
    /// </summary>
    public void RecordDamageDealt(float damage, EntityType targetType)
    {
        // Only track damage dealt to bosses
        if (targetType == EntityType.BOSS_MONSTER && isTrackingBossFight)
        {
            currentRun.RecordDamage(damage);
        }
    }

    /// <summary>
    /// Starts a new run. Resets all tracking data.
    /// </summary>
    public void StartNewRun()
    {
        currentRun.Reset();
        isTrackingBossFight = false;
        currentBossName = string.Empty;
        Debug.Log("RunTracker: Started new run");
    }

    /// <summary>
    /// Ends the current run and calculates final score.
    /// </summary>
    public int EndRun()
    {
        if (isTrackingBossFight)
        {
            currentRun.EndBossFight();
            isTrackingBossFight = false;
        }

        int finalScore = ScoreCalculator.CalculateScore(currentRun);
        ScoreBreakdown breakdown = ScoreCalculator.GetScoreBreakdown(currentRun);
        
        Debug.Log($"=== Run Complete ===");
        Debug.Log($"Bosses Defeated: {breakdown.bossesDefeated}");
        Debug.Log($"Total Damage: {currentRun.totalDamageDealt:F0}");
        Debug.Log($"Damage Score: {breakdown.damageScore}");
        Debug.Log($"Time Score: {breakdown.timeScore}");
        Debug.Log($"Final Score: {breakdown.totalScore}");
        
        return finalScore;
    }

    /// <summary>
    /// Gets the current score without ending the run.
    /// </summary>
    public int GetCurrentScore()
    {
        return ScoreCalculator.CalculateScore(currentRun);
    }

    /// <summary>
    /// Handler for boss fight start event.
    /// </summary>
    private void OnBossFightStart(OnBossFightStartEvent e)
    {
        if (isTrackingBossFight)
        {
            // End previous fight if still tracking
            currentRun.EndBossFight();
        }

        currentBossName = e.bossName ?? "Unknown Boss";
        currentRun.StartBossFight(currentBossName);
        isTrackingBossFight = true;
    }

    /// <summary>
    /// Handler for boss fight end event.
    /// </summary>
    private void OnBossFightEnd(OnBossFightEndEvent e)
    {
        if (isTrackingBossFight)
        {
            currentRun.EndBossFight();
            isTrackingBossFight = false;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        // Unregister events using stored bindings
        if (bossStartBinding != null)
        {
            EventBus<OnBossFightStartEvent>.Unregister(bossStartBinding);
        }

        if (bossEndBinding != null)
        {
            EventBus<OnBossFightEndEvent>.Unregister(bossEndBinding);
        }
    }
}
