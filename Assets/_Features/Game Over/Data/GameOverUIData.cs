using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Data class for the Game Over UI that supports UI Toolkit data binding.
/// This class converts ScoreSummaryData into formatted strings for display.
/// </summary>
[CreateAssetMenu(fileName = "new GameOverUIData", menuName = "System/UI Toolkit/Game Over Data", order = 1)]
public class GameOverUIData : ScriptableObject
{
    [ReadOnly] public string totalScoreText;
    [ReadOnly] public string damageScoreText;
    [ReadOnly] public string timeScoreText;
    [ReadOnly] public string bossesDefeatedText;
    [ReadOnly] public string totalDamageText;
    [ReadOnly] public string timeSurvivedText;

    /// <summary>
    /// Updates the UI data from a ScoreSummaryData object.
    /// </summary>
    public void UpdateFromScoreSummary(ScoreSummaryData data)
    {
        if (data == null)
        {
            Debug.LogWarning("GameOverUIData: ScoreSummaryData is null");
            ClearData();
            return;
        }

        totalScoreText = $"Final Score: {data.totalScore:N0}";
        damageScoreText = $"Damage Score: {data.damageScore:N0}";
        timeScoreText = $"Time Score: {data.timeScore:N0}";
        bossesDefeatedText = $"Bosses Defeated: {data.bossesDefeated}";
        totalDamageText = $"Total Damage: {data.totalDamageDealt:F0}";
        timeSurvivedText = $"Time Survived: {FormatTime(data.totalTimeSurvived)}";
    }

    /// <summary>
    /// Clears all data fields.
    /// </summary>
    public void ClearData()
    {
        totalScoreText = "Final Score: 0";
        damageScoreText = "Damage Score: 0";
        timeScoreText = "Time Score: 0";
        bossesDefeatedText = "Bosses Defeated: 0";
        totalDamageText = "Total Damage: 0";
        timeSurvivedText = "Time Survived: 00:00";
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
}
