using Sirenix.OdinInspector;
using System;
using UnityEngine;

/// <summary>
/// For storing dynamic stat data. Useful for representing stats as a dynamic object.
/// </summary>
[Serializable]
public class StatContainer
{
    public IStatMediator StatMediator { get; private set; }
    [SerializeField, Required] private SO_StatContainer baseStats;

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
    public float Health
    {
        get
        {
            var q = new StatQuery(Stat.HP, baseStats.health);
            StatMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float MaxHealth
    {
        get
        {
            var q = new StatQuery(Stat.MAX_HP, baseStats.maxHealth);
            StatMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float Mana
    {
        get
        {
            var q = new StatQuery(Stat.MP, baseStats.mana);
            StatMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float MaxMana
    {
        get
        {
            var q = new StatQuery(Stat.MAX_MP, baseStats.maxMana);
            StatMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float Stamina
    {
        get
        {
            var q = new StatQuery(Stat.SP, baseStats.stamina);
            StatMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float MaxStamina
    {
        get
        {
            var q = new StatQuery(Stat.MAX_SP, baseStats.maxStamina);
            StatMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float Attack
    {
        get
        {
            var q = new StatQuery(Stat.ATK, baseStats.attack);
            StatMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float Defense
    {
        get
        {
            var q = new StatQuery(Stat.DEF, baseStats.defense);
            StatMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float Dexterity
    {
        get
        {
            var q = new StatQuery(Stat.DEX, baseStats.dexterity);
            StatMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float Speed
    {
        get
        {
            var q = new StatQuery(Stat.SPD, baseStats.speed);
            StatMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    
    // TODO: currently hard coded param. Not using modifiers. See if we need to.
    public float Gold
    {
        get
        {
            //var q = new StatQuery(Stat.GOLD, baseStats.gold);
            //statMediator.CalculateFinalStat(q);
            //return q.Value;
            return baseStats.gold;
        }
        set
        {
            baseStats.gold = value;
            //OnStatChange?.Invoke();
        }
    }
    #endregion

    public float GetStatValue(Stat stat)
    {
        switch (stat)
        {
            case Stat.HP:
                return Health;
            case Stat.MAX_HP:
                return MaxHealth;
            case Stat.MP:
                return Mana;
            case Stat.MAX_MP:
                return MaxMana;
            case Stat.SP:
                return Stamina;
            case Stat.MAX_SP:
                return MaxStamina;
            case Stat.ATK:
                return Attack;
            case Stat.DEF:
                return Defense;
            case Stat.SPD:
                return Speed;
            case Stat.GOLD:
                return Gold;
            default:
                return 0;
        }
    }
}
