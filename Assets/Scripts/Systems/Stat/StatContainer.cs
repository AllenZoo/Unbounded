using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public class StatContainer
{
    public IStatMediator StatMediator { get; private set; }

    [SerializeField, Required]
    private SO_StatContainer baseStats;

    public StatContainer(SO_StatContainer baseStats)
    {
        if (baseStats == null)
        {
            throw new ArgumentNullException(nameof(baseStats));
        }

        this.baseStats = baseStats;
        this.StatMediator = new StatMediator();
    }

    public void Init()
    {
        if (baseStats == null)
        {
            throw new ArgumentNullException(nameof(baseStats));
        }
        this.StatMediator = new StatMediator();
    }

    #region Stats
    public float Health => GetModifiedStat(Stat.HP, baseStats.health);
    public float MaxHealth => GetModifiedStat(Stat.MAX_HP, baseStats.maxHealth);
    public float Attack => GetModifiedStat(Stat.ATK, baseStats.attack);
    public float Defense => GetModifiedStat(Stat.DEF, baseStats.defense);
    public float Dexterity => GetModifiedStat(Stat.DEX, baseStats.dexterity);
    public float Speed => GetModifiedStat(Stat.SPD, baseStats.speed);

    public float Gold
    {
        get => baseStats.gold;
        set => baseStats.gold = value;
    }

    private float GetModifiedStat(Stat stat, float baseValue)
    {
        var query = new StatQuery(stat, baseValue);
        StatMediator.CalculateFinalStat(query);
        return query.Value;
    }
    #endregion

    public float GetStatValue(Stat stat)
    {
        return stat switch
        {
            Stat.HP => Health,
            Stat.MAX_HP => MaxHealth,
            Stat.ATK => Attack,
            Stat.DEF => Defense,
            Stat.DEX => Dexterity,
            Stat.SPD => Speed,
            Stat.GOLD => Gold,
            _ => 0f
        };
    }

    /// <summary>
    /// Util function to get a dictionary of all non zero stats for item descriptor purposes.
    /// </summary>
    /// <returns></returns>
    public Dictionary<Stat, float> GetNonZeroStats()
    {
        var result = new Dictionary<Stat, float>();

        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            float value = GetStatValue(stat);
            if (Mathf.Abs(value) > Mathf.Epsilon)
            {
                result.Add(stat, value);
            }
        }

        return result;
    }

}
