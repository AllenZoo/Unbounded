using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles generating commissions based on some parameters.
/// </summary>
public class CommissionGenerator
{
    public float commissionDifficultyMultiplier = 1.0f;
    public float commissionRewardMultiplier = 1.0f;

    public CommissionGenerator(float commissionDifficultyMultiplier, float commissionRewardMultiplier)
    {
        this.commissionDifficultyMultiplier = commissionDifficultyMultiplier;
        this.commissionRewardMultiplier = commissionRewardMultiplier;
    }

    /// <summary>
    /// Temporary function to get a test commission for testing purposes.
    /// </summary>
    /// <returns></returns>
    public Commission GetTestCommission()
    {
        Dictionary<Stat, int> statRequirements = new Dictionary<Stat, int>();
        statRequirements.Add(Stat.ATK, 3);
        statRequirements.Add(Stat.DEX, 2);
        return new Commission("Help Kullervo forge 'DEATH REAPER'", "Kullervo needs to kill things >:)", 10, 1, 10, EquipmentType.SWORD, statRequirements, CommissionStatus.PENDING);
    }

    /// <summary>
    /// Generates a random PENDING commission.
    /// </summary>
    /// <returns></returns>
    public Commission GenerateCommission()
    {
        // Generate a random commission

        return null;
    }

    public Commission GenerateCommission(EquipmentType equipmentType, int difficulty)
    {
        // Generate a commission based on the equipment type and difficulty
        return null;
    }

    private EquipmentType GenerateRandomEquipmentType()
    {
        Array values = Enum.GetValues(typeof(EquipmentType));
        System.Random random = new System.Random();
        EquipmentType randomEquipmentType = (EquipmentType) values.GetValue(random.Next(values.Length));
        return randomEquipmentType;
    }

    private Dictionary<Stat, int> GenerateRequiredStats(int difficulty)
    {
        Dictionary<Stat, int> statRequirements = new Dictionary<Stat, int>();
        // Generate stat requirements based on the difficulty
        // TODO:
        // Some ideas:
        //   Difficulty ranges from 1 to 5
        //   For each difficulty level, we have a set # of stats that are required.
        //   For example:
        //     Difficulty 1: 5-8 stats
        //     Difficulty 2: 8-12 stats
        //     Difficulty 3: 12-15 stats
        //     Difficulty 4: 15-18 stats
        //     Difficulty 5: 18-20 stats
        // 
        // Required stats may also get scaled based on an additional seperate difficulty multiplier (commissionDifficultyMultiplier scales with # of commissions
        // completed, commissionRewardMultiplier scales with # of commissions completed)


        return statRequirements;
    }

    private int GenerateReward(int difficulty)
    {
        // Generate a reward based on the difficulty
        return 1;
    }

    private String GeenerateTitle(EquipmentType equipmentType, int difficulty)
    {
        // Generate a title based on the equipment type and difficulty
        return "";
    }

    private String GenerateDescription(EquipmentType equipmentType, int difficulty)
    {
        // Generate a description based on the equipment type and difficulty
        return "";
    }

    private int GenerateTimeLimit(int difficulty)
    {
        // Generate a time limit based on the difficulty
        return 1;
    }
}
