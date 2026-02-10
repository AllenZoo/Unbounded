using System;
using UnityEngine;

/// <summary>
/// Data container for the game over score summary.
/// This object is passed to the UI for display and contains all relevant run statistics.
/// </summary>
[Serializable]
public class ScoreSummaryData
{
    public int totalScore;
    public int damageScore;
    public int timeScore;
    public int bossesDefeated;
    public float totalDamageDealt;
    public float totalTimeSurvived;
    
    /// <summary>
    /// Creates a ScoreSummaryData from RunData using the existing ScoreCalculator.
    /// </summary>
    public static ScoreSummaryData FromRunData(RunData runData)
    {
        if (runData == null)
        {
            Debug.LogWarning("RunData is null when creating ScoreSummaryData");
            return new ScoreSummaryData();
        }

        ScoreBreakdown breakdown = ScoreCalculator.GetScoreBreakdown(runData);
        
        // Calculate total time survived across all boss fights
        float totalTime = 0f;
        foreach (var bossFight in runData.bossFights)
        {
            totalTime += bossFight.fightDuration;
        }

        return new ScoreSummaryData
        {
            totalScore = breakdown.totalScore,
            damageScore = breakdown.damageScore,
            timeScore = breakdown.timeScore,
            bossesDefeated = breakdown.bossesDefeated,
            totalDamageDealt = runData.totalDamageDealt,
            totalTimeSurvived = totalTime
        };
    }
}
