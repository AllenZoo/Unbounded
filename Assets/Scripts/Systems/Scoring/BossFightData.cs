using System;
using UnityEngine;

/// <summary>
/// Stores data for a single boss fight within a run.
/// Tracks timing and damage dealt during the fight.
/// </summary>
[Serializable]
public class BossFightData
{
    public string bossName;
    public float damageDealt;
    public float fightDuration; // in seconds
    public float startTime;
    
    public BossFightData(string bossName)
    {
        this.bossName = bossName;
        this.damageDealt = 0f;
        this.fightDuration = 0f;
        this.startTime = 0f;
    }

    /// <summary>
    /// Starts the timer for this boss fight.
    /// </summary>
    public void StartFight()
    {
        startTime = Time.time;
    }

    /// <summary>
    /// Ends the timer for this boss fight and calculates duration.
    /// </summary>
    public void EndFight()
    {
        if (startTime > 0)
        {
            fightDuration = Time.time - startTime;
        }
    }

    /// <summary>
    /// Adds damage to the total damage dealt in this fight.
    /// </summary>
    public void AddDamage(float damage)
    {
        damageDealt += damage;
    }
}
