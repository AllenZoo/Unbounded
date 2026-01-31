using UnityEngine;

/// <summary>
/// Calculates final score based on run statistics.
/// Formula is extensible to easily add new scoring factors.
/// </summary>
public static class ScoreCalculator
{
    // Scoring weights and configuration
    private const float DAMAGE_WEIGHT = 1.0f;
    private const float TIME_BONUS_BASE = 1000f;
    private const float TIME_PENALTY_PER_SECOND = 0.5f;
    
    // Default optimal time - can be overridden per calculation
    public const float DEFAULT_OPTIMAL_BOSS_FIGHT_TIME = 60f; // 1 minute optimal time
    
    /// <summary>
    /// Calculates the final score for a run based on damage dealt and fight times.
    /// </summary>
    /// <param name="runData">The run data containing all boss fights and damage</param>
    /// <param name="optimalTime">Optional optimal fight time in seconds (default: 60s)</param>
    /// <returns>Final score as an integer</returns>
    public static int CalculateScore(RunData runData, float optimalTime = DEFAULT_OPTIMAL_BOSS_FIGHT_TIME)
    {
        if (runData == null)
        {
            Debug.LogWarning("RunData is null. Returning score of 0.");
            return 0;
        }

        float totalScore = 0f;

        // Component 1: Damage score (direct contribution)
        float damageScore = CalculateDamageScore(runData.totalDamageDealt);
        totalScore += damageScore;

        // Component 2: Time bonus/penalty per boss
        float timeScore = CalculateTimeScore(runData, optimalTime);
        totalScore += timeScore;

        // Ensure score is never negative
        totalScore = Mathf.Max(0f, totalScore);

        Debug.Log($"Score Calculation - Damage: {damageScore:F0}, Time: {timeScore:F0}, Total: {totalScore:F0}");

        return Mathf.RoundToInt(totalScore);
    }

    /// <summary>
    /// Calculates score contribution from total damage dealt.
    /// </summary>
    private static float CalculateDamageScore(float totalDamage)
    {
        return totalDamage * DAMAGE_WEIGHT;
    }

    /// <summary>
    /// Calculates score contribution from boss fight times.
    /// Faster kills result in higher scores.
    /// </summary>
    /// <param name="optimalTime">The optimal fight time in seconds for maximum bonus</param>
    private static float CalculateTimeScore(RunData runData, float optimalTime)
    {
        float totalTimeScore = 0f;

        foreach (var bossFight in runData.bossFights)
        {
            if (bossFight.fightDuration <= 0) continue;

            // Calculate time bonus/penalty
            // Faster than optimal time = bonus
            // Slower than optimal time = penalty
            float timeDifference = bossFight.fightDuration - optimalTime;
            float timeBonus = TIME_BONUS_BASE - (timeDifference * TIME_PENALTY_PER_SECOND);
            
            // Cap the bonus/penalty to reasonable values
            timeBonus = Mathf.Clamp(timeBonus, -500f, 2000f);
            
            totalTimeScore += timeBonus;
        }

        return totalTimeScore;
    }

    /// <summary>
    /// Gets a score breakdown for display purposes.
    /// </summary>
    /// <param name="optimalTime">Optional optimal fight time in seconds (default: 60s)</param>
    public static ScoreBreakdown GetScoreBreakdown(RunData runData, float optimalTime = DEFAULT_OPTIMAL_BOSS_FIGHT_TIME)
    {
        if (runData == null)
        {
            return new ScoreBreakdown();
        }

        return new ScoreBreakdown
        {
            damageScore = Mathf.RoundToInt(CalculateDamageScore(runData.totalDamageDealt)),
            timeScore = Mathf.RoundToInt(CalculateTimeScore(runData, optimalTime)),
            bossesDefeated = runData.bossFights.Count,
            totalScore = CalculateScore(runData, optimalTime)
        };
    }
}

/// <summary>
/// Breakdown of score components for display.
/// </summary>
[System.Serializable]
public struct ScoreBreakdown
{
    public int damageScore;
    public int timeScore;
    public int bossesDefeated;
    public int totalScore;
}
