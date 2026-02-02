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
        // Note: Player damage tracking is handled through BossDamageTracker component
        // attached to boss entities. This listens to boss OnDamagedEvent and reports
        // damage to RunTracker automatically.
        Debug.Log("RunTracker initialized. Boss damage tracking via BossDamageTracker components.");
    }

    /// <summary>
    /// Registers to track damage dealt by the player's attacks.
    /// Note: Damage tracking is primarily handled by BossDamageTracker components on boss entities.
    /// This method is kept for potential future use with direct player tracking.
    /// </summary>
    public void RegisterPlayerDamageTracking(LocalEventHandler playerLEH)
    {
        // Currently unused - damage tracking happens through BossDamageTracker on bosses
        // This method can be extended in the future if direct player attack tracking is needed
        Debug.Log("RunTracker: Player damage tracking registration (currently handled via BossDamageTracker)");
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

    protected void OnEnable()
    {
        // Re-register events in case of re-enable
        if (bossStartBinding != null)
        {
            EventBus<OnBossFightStartEvent>.Register(bossStartBinding);
        }
        if (bossEndBinding != null)
        {
            EventBus<OnBossFightEndEvent>.Register(bossEndBinding);
        }
    }

    protected void OnDisable()
    {
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

    protected void OnDestroy()
    {
        
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

    ////TODO: for testing only, remove later
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.F5))
    //    {
    //        StartNewRun();
    //    }
    //    if (Input.GetKeyDown(KeyCode.F6))
    //    {
    //        EndRun();
    //    }

    //}
}
