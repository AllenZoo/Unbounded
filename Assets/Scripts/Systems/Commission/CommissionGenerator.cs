using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles generating commissions based on some parameters.
/// </summary>
public class CommissionGenerator
{
    public static readonly List<string> COOL_WEAPON_NAMES = new List<string>
{
    "Shadowfang",
    "Doombringer",
    "Nightfall",
    "Bloodbane",
    "Stormreaver",
    "Oblivion Edge",
    "Frostfang",
    "Soulrender",
    "Dragonfang",
    "Abyssal Wrath",
    "Thunderstrike",
    "Eclipse Scythe",
    "Venomfang",
    "Demonhowl",
    "Celestial Cleaver",
    "Chaos Reaver",
    "Hellfire Saber",
    "Phantom Dagger",
    "Voidpiercer",
    "Ruinblade",
    "Inferno Pike",
    "Lightningfang",
    "Deathwhisper",
    "Onyx Slayer",
    "Warbringer",
    "Seraphic Edge",
    "Darkstar Halberd",
    "Runeblade of Eternity",
    "Echo of the Fallen",
    "Skybreaker"
};

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
        EquipmentType equipmentType = GenerateRandomEquipmentType();
        int difficulty = GenerateRandomDifficulty();
        return GenerateCommission(equipmentType, difficulty);
    }

    public List<Commission> GenerateCommissions(int num)
    {
        List<Commission> commissions = new List<Commission>();
        if (num <= 0) return commissions;

        for (int i = 0; i < num; i++)
        {
            commissions.Add(GenerateCommission());
        }
        return commissions;
    }

    public Commission GenerateCommission(EquipmentType equipmentType, int difficulty)
    {
        // Generate a commission based on the equipment type and difficulty
        Commission commission = new Commission(
                       GenerateTitle(equipmentType, difficulty),
                       GenerateDescription(equipmentType, difficulty),
                       GenerateReward(difficulty),
                       difficulty,
                       GenerateTimeLimit(difficulty),
                       equipmentType,
                       GenerateRequiredStats(difficulty),
                       CommissionStatus.PENDING);
        return commission;
    }


    #region Commission Generation Helper Functions
    private EquipmentType GenerateRandomEquipmentType()
    {
        Array values = Enum.GetValues(typeof(EquipmentType));
        System.Random random = new System.Random();
        EquipmentType randomEquipmentType = (EquipmentType) values.GetValue(random.Next(values.Length));
        return randomEquipmentType;
    }

    /// <summary>
    /// Generates a difficulty level from 1-5.
    /// </summary>
    /// <returns></returns>
    private int GenerateRandomDifficulty()
    {
        return UnityEngine.Random.Range(1, 6);
    }

    /// <summary>
    ///  Generate stat requirements based on the difficulty
    ///
    ///  Difficulty ranges from 1 to 5
    ///  For each difficulty level, we have a set # of stats that are required.
    ///  For example:
    ///     Difficulty 1: 5-8 stats
    ///     Difficulty 2: 8-12 stats
    ///     Difficulty 3: 12-15 stats
    ///     Difficulty 4: 15-18 stats
    ///     Difficulty 5: 18-20 stats
    /// 
    /// # of stats required also get scaled based on the commissionDifficultyMultiplier.
    /// 
    /// </summary>
    /// <param name="difficulty"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private Dictionary<Stat, int> GenerateRequiredStats(int difficulty)
    {
        Dictionary<Stat, int> statRequirements = new Dictionary<Stat, int>();
        // Define stat ranges for each difficulty
        int minStats = 0;
        int maxStats = 0;

        switch (difficulty)
        {
            case 1:
                minStats = 5;
                maxStats = 8;
                break;
            case 2:
                minStats = 8;
                maxStats = 12;
                break;
            case 3:
                minStats = 12;
                maxStats = 15;
                break;
            case 4:
                minStats = 15;
                maxStats = 18;
                break;
            case 5:
                minStats = 18;
                maxStats = 20;
                break;
            default:
                throw new ArgumentException("Invalid difficulty level");
        }

        // Generate a random total number of stats required within the range
        System.Random random = new System.Random();
        int totalStatsRequired = random.Next(minStats, maxStats + 1);

        // Scale the total stats required based on the difficulty multipliers
        totalStatsRequired = (int)(totalStatsRequired * commissionDifficultyMultiplier * commissionRewardMultiplier);

        // Distribute stats into the different stats randomly
        for (int i = 0; i < totalStatsRequired; i++)
        {
            Stat randomStat = GetRandomEquipmentStatType();
            if (statRequirements.ContainsKey(randomStat))
            {
                statRequirements[randomStat]++;
            }
            else
            {
                statRequirements[randomStat] = 1;
            }
        }

        return statRequirements;
    }

    /// <summary>
    /// Generates a reward based on the difficulty level and the commissionRewardMultiplier.
    /// 
    /// Base:
    /// Difficulty 1: 5-10
    /// Difficulty 2: 10-20
    /// Difficulty 3: 20-30
    /// Difficulty 4: 30-40
    /// Difficulty 5: 40-50
    /// 
    /// Final:
    ///   Base * commissionRewardMultiplier
    /// 
    /// </summary>
    /// <param name="difficulty"></param>
    /// <returns></returns>
    private int GenerateReward(int difficulty)
    {
        // Check difficulty bounds
        if (difficulty < 1 || difficulty > 5)
        {
            Debug.LogError("Invalid difficulty level for generating reward!");
            return 0;
        }

        // Generate a reward based on the difficulty (range 1-5) and the commissionRewardMultiplier
        int reward = 0;
        switch (difficulty)
        {
            // Range from 5-10
            case 1:
                reward = UnityEngine.Random.Range(5, 11);
                break;
            // Range from 10-20
            case 2:
                reward = UnityEngine.Random.Range(10, 21); ;
                break;
            // Range from 20-30
            case 3:
                reward = UnityEngine.Random.Range(20, 31); ;
                break;
            // Range from 30-40
            case 4:
                reward = UnityEngine.Random.Range(30, 41); ;
                break;
            // Range from 40-50
            case 5:
                reward = UnityEngine.Random.Range(40, 51);
                break;
            // Should not reach here
            default:
                Debug.LogError("Invalid difficulty level for generating reward!");
                break;
        }

        double unroundedReward = reward * commissionRewardMultiplier;
        reward = (int) Math.Round(unroundedReward);
        return reward;
    }

    // TODO: implement properly
    private String GenerateTitle(EquipmentType equipmentType, int difficulty)
    {
        // Generate a title based on the equipment type and difficulty
        int random = UnityEngine.Random.Range(0, COOL_WEAPON_NAMES.Count - 1);
        return COOL_WEAPON_NAMES[random];
    }

    private String GenerateDescription(EquipmentType equipmentType, int difficulty)
    {
        // Generate a description based on the equipment type and difficulty
        return "You've been tasked to craft a mighty weapon, for uhh killing things. Bad things, ofcourse.";
    }

    /// <summary>
    /// Generates a time limit based on the difficulty level.
    /// 
    /// Base:
    /// Difficulty 1: 7-8 days
    /// Difficulty 2: 6-7 days
    /// Difficulty 3: 5-6 days
    /// Difficulty 4: 4-5 days
    /// Difficulty 5: 2-3 days
    /// 
    /// Final:
    ///   Base * No multiplier (time limit shouldn't be affected by commissionDifficultyMultiplier for now
    private int GenerateTimeLimit(int difficulty)
    {
        // Check difficulty bounds
        if (difficulty < 1 || difficulty > 5)
        {
            Debug.LogError("Invalid difficulty level for generating time limit!");
            return 0;
        }

        // Generate a time limit based on the difficulty (1-5)
        int timeLimit = 0;
        switch (difficulty)
        {
            // Range from 7-8
            case 1:
                timeLimit = UnityEngine.Random.Range(7, 9);
                break;
            // Range from 6-7
            case 2:
                timeLimit = UnityEngine.Random.Range(6, 8);
                break;
            // Range from 5-6
            case 3:
                timeLimit = UnityEngine.Random.Range(5, 7);
                break;
            // Range from 4-5
            case 4:
                timeLimit = UnityEngine.Random.Range(4, 6);
                break;
            // Range from 2-3
            case 5:
                timeLimit = UnityEngine.Random.Range(2, 4);
                break;
            // Should not reach here
            default:
                Debug.LogError("Invalid difficulty level for generating time limit!");
                break;
        }
        return timeLimit;
    }


    /// <summary>
    /// Returns ATK, DEF, SPD, or DEX randomly
    /// </summary>
    /// <returns></returns>
    private Stat GetRandomEquipmentStatType()
    {
        Stat[] statsToPickFrom = { Stat.ATK, Stat.DEF, Stat.SPD, Stat.DEX };
        Stat stat;

        // Randomly select a stat type from the array
        System.Random random = new System.Random();
        stat = statsToPickFrom[random.Next(statsToPickFrom.Length)];
        return stat;
    }
    #endregion
}
