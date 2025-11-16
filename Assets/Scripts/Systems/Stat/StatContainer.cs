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
        health = baseStats.health;
    }

    public void Init()
    {
        if (baseStats == null)
        {
            throw new ArgumentNullException(nameof(baseStats));
        }
        this.StatMediator = new StatMediator();
        health = baseStats.health;
    }

    // Note: Health is special since it's so frequently modified, we just decided to store it without the modifier chain.
    private float health;

    #region Stats
    public float Health { get { return health; } set { health = value; } }
    public float MaxHealth => GetModifiedStat(Stat.MAX_HP, baseStats.maxHealth);
    public float Attack => GetModifiedStat(Stat.ATK, baseStats.attack);
    public float Defense => GetModifiedStat(Stat.DEF, baseStats.defense);
    public float Dexterity => GetModifiedStat(Stat.DEX, baseStats.dexterity);
    public float Speed { 
        get { 
            curSpeed = GetModifiedStat(Stat.SPD, baseStats.speed);
            return curSpeed;
        } 
    }

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

    // For debugging purposes.
    [SerializeField, ReadOnly] private float curSpeed = 0;
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

    /// <summary>
    /// Util function to get a diff in stats between two stat containers.
    /// 
    /// Returns "this - other"
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public StatContainer Diff(StatContainer other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        // Create a temporary "diff" SO_StatContainer (not saved to project).
        var diffStats = ScriptableObject.CreateInstance<SO_StatContainer>();

        // Fill with differences
        diffStats.health = this.Health - other.Health;
        diffStats.maxHealth = this.MaxHealth - other.MaxHealth;
        diffStats.attack = this.Attack - other.Attack;
        diffStats.defense = this.Defense - other.Defense;
        diffStats.dexterity = this.Dexterity - other.Dexterity;
        diffStats.speed = this.Speed - other.Speed;
        diffStats.gold = this.Gold - other.Gold;

        // Wrap it in a StatContainer and return
        var diffContainer = new StatContainer(diffStats);
        diffContainer.Init(); // fresh mediator
        return diffContainer;
    }

}
