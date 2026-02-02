using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks statistics for a single game run.
/// Stores damage dealt and time spent on each boss fight (max 9 bosses per run).
/// </summary>
[Serializable]
public class RunData
{
    private const int MAX_BOSSES = 9;
    private const int NO_ACTIVE_BOSS = -1;
    
    public List<BossFightMetrics> bossFights;
    public float totalDamageDealt;
    public int currentBossIndex;
    
    public RunData()
    {
        bossFights = new List<BossFightMetrics>(MAX_BOSSES);
        totalDamageDealt = 0f;
        currentBossIndex = NO_ACTIVE_BOSS;
    }

    /// <summary>
    /// Starts tracking a new boss fight.
    /// </summary>
    public void StartBossFight(string bossName)
    {
        if (bossFights.Count >= MAX_BOSSES)
        {
            Debug.LogWarning($"Maximum boss count ({MAX_BOSSES}) reached. Cannot track more boss fights.");
            return;
        }

        BossFightMetrics newFight = new BossFightMetrics(bossName);
        newFight.StartFight();
        bossFights.Add(newFight);
        currentBossIndex = bossFights.Count - 1;
        
        Debug.Log($"Started tracking boss fight: {bossName} (Boss #{currentBossIndex + 1})");
    }

    /// <summary>
    /// Ends the current boss fight tracking.
    /// </summary>
    public void EndBossFight()
    {
        if (currentBossIndex >= 0 && currentBossIndex < bossFights.Count)
        {
            bossFights[currentBossIndex].EndFight();
            Debug.Log($"Ended boss fight: {bossFights[currentBossIndex].bossName}, Duration: {bossFights[currentBossIndex].fightDuration:F2}s");
        }
    }

    /// <summary>
    /// Records damage dealt during the current boss fight.
    /// </summary>
    public void RecordDamage(float damage)
    {
        if (damage <= 0) return;

        totalDamageDealt += damage;

        // Add to current boss fight if one is active
        if (currentBossIndex >= 0 && currentBossIndex < bossFights.Count)
        {
            bossFights[currentBossIndex].AddDamage(damage);
        }
    }

    /// <summary>
    /// Gets the current boss fight data if one is active.
    /// </summary>
    public BossFightMetrics GetCurrentBossFight()
    {
        if (currentBossIndex >= 0 && currentBossIndex < bossFights.Count)
        {
            return bossFights[currentBossIndex];
        }
        return null;
    }

    /// <summary>
    /// Resets all run data for a new run.
    /// </summary>
    public void Reset()
    {
        bossFights.Clear();
        totalDamageDealt = 0f;
        currentBossIndex = NO_ACTIVE_BOSS;
    }
}
