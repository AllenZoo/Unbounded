using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI component that displays the game over score summary.
/// This component only displays data - it does not calculate scores or manage game state.
/// </summary>
public class GameOverUI : PageUI
{
    [Header("Score Display")]
    [SerializeField] private TextMeshProUGUI totalScoreText;
    [SerializeField] private TextMeshProUGUI damageScoreText;
    [SerializeField] private TextMeshProUGUI timeScoreText;
    [SerializeField] private TextMeshProUGUI bossesDefeatedText;
    
    [Header("Statistics Display")]
    [SerializeField] private TextMeshProUGUI totalDamageText;
    [SerializeField] private TextMeshProUGUI timeSurvivedText;
    
    [Header("Action Buttons (Optional)")]
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    private EventBinding<OnGameOverEvent> gameOverBinding;

    protected override void Awake()
    {
        base.Awake();
        
        // Register for game over event
        gameOverBinding = new EventBinding<OnGameOverEvent>(OnGameOver);
        EventBus<OnGameOverEvent>.Register(gameOverBinding);
        
        // Setup button listeners if they exist
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryClicked);
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
        
        // Start hidden
        ToggleVisibility(false);
    }

    private void OnGameOver(OnGameOverEvent e)
    {
        DisplayScoreSummary(e.scoreSummary);
        MoveToTop();
    }

    /// <summary>
    /// Displays the score summary on the UI.
    /// </summary>
    public void DisplayScoreSummary(ScoreSummaryData data)
    {
        if (data == null)
        {
            Debug.LogWarning("ScoreSummaryData is null");
            return;
        }

        // Display scores
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Final Score: {data.totalScore:N0}";
        }
        
        if (damageScoreText != null)
        {
            damageScoreText.text = $"Damage Score: {data.damageScore:N0}";
        }
        
        if (timeScoreText != null)
        {
            timeScoreText.text = $"Time Score: {data.timeScore:N0}";
        }
        
        if (bossesDefeatedText != null)
        {
            bossesDefeatedText.text = $"Bosses Defeated: {data.bossesDefeated}";
        }

        // Display statistics
        if (totalDamageText != null)
        {
            totalDamageText.text = $"Total Damage: {data.totalDamageDealt:F0}";
        }
        
        if (timeSurvivedText != null)
        {
            timeSurvivedText.text = $"Time Survived: {FormatTime(data.totalTimeSurvived)}";
        }

        Debug.Log($"GameOverUI: Displayed score summary - Total Score: {data.totalScore}");
    }

    /// <summary>
    /// Formats time in seconds to a readable format (MM:SS).
    /// </summary>
    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:D2}:{secs:D2}";
    }

    private void OnRetryClicked()
    {
        Debug.Log("GameOverUI: Retry button clicked");
        
        // Trigger a new run through the game manager
        if (GameManagerComponent.Instance != null)
        {
            // Close the game over UI
            ClosePage();
            GameManagerComponent.Instance.StartNewRun();
        }
        else
        {
            Debug.LogError("GameOverUI: Cannot retry - GameManagerComponent.Instance is null");
        }
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("GameOverUI: Main menu button clicked");
        // Close the game over UI
        ClosePage();
        
        // TODO: Implement return to main menu functionality
        // This would typically involve loading the main menu scene
        Debug.LogWarning("Main menu functionality not yet implemented");
    }

    private void OnDestroy()
    {
        // Unregister event
        if (gameOverBinding != null)
        {
            EventBus<OnGameOverEvent>.Unregister(gameOverBinding);
        }
        
        // Remove button listeners
        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(OnRetryClicked);
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        }
    }
}
