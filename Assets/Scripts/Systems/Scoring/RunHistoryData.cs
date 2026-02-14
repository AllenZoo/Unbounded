using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data class for tracking individual run history records.
/// Stores score, weapons used, duration, and timestamp for a completed run.
/// </summary>
[Serializable]
public class RunHistoryData
{
    public int score;
    public List<WeaponUsageData> weaponsUsed;
    public float duration;
    public string timestamp;
    public int bossesDefeated;
    public float totalDamageDealt;

    public RunHistoryData()
    {
        weaponsUsed = new List<WeaponUsageData>();
    }

    public RunHistoryData(ScoreSummaryData scoreSummary, List<WeaponUsageData> weapons, string time)
    {
        score = scoreSummary.totalScore;
        bossesDefeated = scoreSummary.bossesDefeated;
        totalDamageDealt = scoreSummary.totalDamageDealt;
        duration = scoreSummary.totalTimeSurvived;
        weaponsUsed = weapons ?? new List<WeaponUsageData>();
        timestamp = time;
    }
}

/// <summary>
/// Data class for tracking weapon usage during a run.
/// Stores weapon ID and name for persistence.
/// </summary>
[Serializable]
public class WeaponUsageData
{
    public string weaponId;
    public string weaponName;
    public float equipTime;

    public WeaponUsageData(string id, string name, float time)
    {
        weaponId = id;
        weaponName = name;
        equipTime = time;
    }
}
