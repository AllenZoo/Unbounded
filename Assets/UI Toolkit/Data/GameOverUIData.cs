using UnityEngine;

/// <summary>
/// Data class for the Game Over UI that supports UI Toolkit data binding.
/// This class converts ScoreSummaryData into formatted strings for display.
/// </summary>
public class GameOverUIData : ScriptableObject
{
    public string totalScoreText;
    public string damageScoreText;
    public string timeScoreText;
    public string bossesDefeatedText;
    public string totalDamageText;
    public string timeSurvivedText;

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
