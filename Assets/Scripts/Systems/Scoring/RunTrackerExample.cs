using UnityEngine;

/// <summary>
/// Example script demonstrating how to use the RunTracker scoring system.
/// This can be used for testing or as a reference implementation.
/// </summary>
public class RunTrackerExample : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private bool runExampleOnStart = false;
    
    private void Start()
    {
        if (runExampleOnStart)
        {
            RunExample();
        }
    }

    /// <summary>
    /// Example demonstrating the full lifecycle of run tracking.
    /// </summary>
    public void RunExample()
    {
        Debug.Log("=== RunTracker Example Start ===");

        // 1. Start a new run
        if (RunTracker.Instance != null)
        {
            RunTracker.Instance.StartNewRun();
            Debug.Log("Started new run");
        }
        else
        {
            Debug.LogError("RunTracker.Instance is null! Make sure RunTracker component exists in scene.");
            return;
        }

        // 2. Simulate first boss fight
        SimulateBossFight("Boss 1", 1500f, 45f); // 1500 damage, 45 seconds (fast kill)

        // 3. Simulate second boss fight
        SimulateBossFight("Boss 2", 2000f, 75f); // 2000 damage, 75 seconds (slow kill)

        // 4. Simulate third boss fight
        SimulateBossFight("Boss 3", 1800f, 60f); // 1800 damage, 60 seconds (optimal time)

        // 5. Check current score
        int currentScore = RunTracker.Instance.GetCurrentScore();
        Debug.Log($"Current score after 3 bosses: {currentScore}");

        // 6. Get score breakdown
        ScoreBreakdown breakdown = ScoreCalculator.GetScoreBreakdown(RunTracker.Instance.CurrentRun);
        Debug.Log($"Score Breakdown:");
        Debug.Log($"  - Bosses Defeated: {breakdown.bossesDefeated}");
        Debug.Log($"  - Damage Score: {breakdown.damageScore}");
        Debug.Log($"  - Time Score: {breakdown.timeScore}");
        Debug.Log($"  - Total Score: {breakdown.totalScore}");

        // 7. End run
        int finalScore = RunTracker.Instance.EndRun();
        Debug.Log($"Run ended with final score: {finalScore}");

        Debug.Log("=== RunTracker Example End ===");
    }

    /// <summary>
    /// Simulates a complete boss fight for testing purposes.
    /// </summary>
    private void SimulateBossFight(string bossName, float damageDealt, float fightDuration)
    {
        Debug.Log($"Simulating boss fight: {bossName}");

        // Start boss fight
        EventBus<OnBossFightStartEvent>.Raise(new OnBossFightStartEvent { bossName = bossName });

        // Simulate damage dealt during fight
        // Break it into chunks to simulate realistic combat
        int damageChunks = 10;
        float damagePerChunk = damageDealt / damageChunks;
        
        for (int i = 0; i < damageChunks; i++)
        {
            RunTracker.Instance.RecordDamageDealt(damagePerChunk, EntityType.BOSS_MONSTER);
        }

        // Manually adjust fight duration (normally handled by BossFightData timing)
        var currentFight = RunTracker.Instance.CurrentRun.GetCurrentBossFight();
        if (currentFight != null)
        {
            currentFight.fightDuration = fightDuration;
        }

        // End boss fight
        EventBus<OnBossFightEndEvent>.Raise(new OnBossFightEndEvent { bossName = bossName });

        Debug.Log($"Boss fight complete: {bossName} - {damageDealt} damage in {fightDuration}s");
    }
}
