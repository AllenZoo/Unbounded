using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Data class for the High Score UI that supports UI Toolkit data binding.
/// This class formats run history data for display in the UI.
/// </summary>
[CreateAssetMenu(fileName = "new HighScoreUIData", menuName = "System/UI Toolkit/High Score Data", order = 2)]
public class HighScoreUIData : ScriptableObject
{
    [ReadOnly] public string highScoreText;
    [ReadOnly] public List<RunHistoryDisplayData> runHistoryList;

    public HighScoreUIData()
    {
        runHistoryList = new List<RunHistoryDisplayData>();
    }

    /// <summary>
    /// Updates the UI data from the RunHistoryManager.
    /// </summary>
    public void UpdateFromRunHistory()
    {
        if (RunHistoryManager.Instance == null)
        {
            Debug.LogWarning("HighScoreUIData: RunHistoryManager.Instance is null");
            ClearData();
            return;
        }

        int highScore = RunHistoryManager.Instance.HighScore;
        List<RunHistoryData> history = RunHistoryManager.Instance.RunHistory;

        highScoreText = $"High Score: {highScore:N0}";

        // Convert run history to display data
        runHistoryList = new List<RunHistoryDisplayData>();
        
        // Display in reverse order (newest first)
        for (int i = history.Count - 1; i >= 0; i--)
        {
            RunHistoryData run = history[i];
            RunHistoryDisplayData displayData = new RunHistoryDisplayData
            {
                score = run.score,
                scoreText = $"Score: {run.score:N0}",
                timestamp = run.timestamp,
                bossesDefeatedText = $"Bosses: {run.bossesDefeated}",
                damageText = $"Damage: {run.totalDamageDealt:F0}",
                durationText = $"Time: {FormatTime(run.duration)}",
                weaponsText = FormatWeaponsList(run.weaponsUsed)
            };
            runHistoryList.Add(displayData);
        }

        Debug.Log($"HighScoreUIData: Updated with {runHistoryList.Count} runs, high score {highScore}");
    }

    /// <summary>
    /// Clears all data fields.
    /// </summary>
    public void ClearData()
    {
        highScoreText = "High Score: 0";
        runHistoryList.Clear();
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

    /// <summary>
    /// Formats the list of weapons used into a readable string.
    /// </summary>
    private string FormatWeaponsList(List<WeaponUsageData> weapons)
    {
        if (weapons == null || weapons.Count == 0)
        {
            return "No weapons recorded";
        }

        // Get unique weapon names
        List<string> uniqueWeaponNames = weapons
            .Select(w => w.weaponName)
            .Distinct()
            .ToList();

        return string.Join(", ", uniqueWeaponNames);
    }
}

/// <summary>
/// Display data structure for a single run history entry.
/// </summary>
[System.Serializable]
public class RunHistoryDisplayData
{
    public int score;
    public string scoreText;
    public string timestamp;
    public string bossesDefeatedText;
    public string damageText;
    public string durationText;
    public string weaponsText;
}
