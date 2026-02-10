using UnityEngine;

/// <summary>
/// Example script demonstrating how to trigger the Game Over UI for testing purposes.
/// Attach this to a GameObject in your scene and press the specified keys to test.
/// 
/// This is for TESTING ONLY - remove from production builds.
/// </summary>
public class GameOverUITester : MonoBehaviour
{
    [Header("Test Controls")]
    [Tooltip("Key to trigger a test game over with sample data")]
    public KeyCode testGameOverKey = KeyCode.F9;
    
    [Tooltip("Key to trigger a test game over with zero score")]
    public KeyCode testZeroScoreKey = KeyCode.F10;
    
    [Header("Test Data")]
    [SerializeField] private int testTotalScore = 12345;
    [SerializeField] private int testDamageScore = 10000;
    [SerializeField] private int testTimeScore = 2345;
    [SerializeField] private int testBossesDefeated = 3;
    [SerializeField] private float testTotalDamage = 10000f;
    [SerializeField] private float testTimeSurvived = 185.5f; // 3:05

    private void Update()
    {
        // Test with sample data
        if (Input.GetKeyDown(testGameOverKey))
        {
            TriggerTestGameOver();
        }
        
        // Test with zero score
        if (Input.GetKeyDown(testZeroScoreKey))
        {
            TriggerTestGameOverZeroScore();
        }
    }

    /// <summary>
    /// Triggers a test game over with the configured sample data.
    /// </summary>
    public void TriggerTestGameOver()
    {
        Debug.Log($"[GameOverUITester] Triggering test game over with sample data (Press {testGameOverKey})");
        
        ScoreSummaryData testData = new ScoreSummaryData
        {
            totalScore = testTotalScore,
            damageScore = testDamageScore,
            timeScore = testTimeScore,
            bossesDefeated = testBossesDefeated,
            totalDamageDealt = testTotalDamage,
            totalTimeSurvived = testTimeSurvived
        };
        
        EventBus<OnGameOverEvent>.Raise(new OnGameOverEvent { scoreSummary = testData });
    }

    /// <summary>
    /// Triggers a test game over with zero score (edge case testing).
    /// </summary>
    public void TriggerTestGameOverZeroScore()
    {
        Debug.Log($"[GameOverUITester] Triggering test game over with zero score (Press {testZeroScoreKey})");
        
        ScoreSummaryData testData = new ScoreSummaryData
        {
            totalScore = 0,
            damageScore = 0,
            timeScore = 0,
            bossesDefeated = 0,
            totalDamageDealt = 0f,
            totalTimeSurvived = 0f
        };
        
        EventBus<OnGameOverEvent>.Raise(new OnGameOverEvent { scoreSummary = testData });
    }

    /// <summary>
    /// Triggers a test game over with high scores (testing number formatting).
    /// </summary>
    public void TriggerTestGameOverHighScore()
    {
        Debug.Log("[GameOverUITester] Triggering test game over with high score");
        
        ScoreSummaryData testData = new ScoreSummaryData
        {
            totalScore = 999999,
            damageScore = 500000,
            timeScore = 499999,
            bossesDefeated = 9,
            totalDamageDealt = 500000f,
            totalTimeSurvived = 543.2f // 9:03
        };
        
        EventBus<OnGameOverEvent>.Raise(new OnGameOverEvent { scoreSummary = testData });
    }

    private void OnGUI()
    {
        // Display test instructions in the game view
        GUILayout.BeginArea(new Rect(10, 10, 300, 120));
        GUILayout.Box("Game Over UI Tester");
        GUILayout.Label($"Press {testGameOverKey} - Test with sample data");
        GUILayout.Label($"Press {testZeroScoreKey} - Test with zero score");
        GUILayout.Label("Or use Inspector buttons");
        GUILayout.EndArea();
    }
}
