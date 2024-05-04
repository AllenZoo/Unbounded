using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatComponent : MonoBehaviour
{
    [SerializeField] private LocalEventHandler localEventHandler;

    public event Action<StatComponent, IStatModifier> OnStatChange;
    [SerializeField] private SO_StatContainer baseStats;
    public float health { get; private set; }
    public float maxHealth { get; private set; }
    public float mana { get; private set; }
    public float maxMana { get; private set; }
    public float stamina { get; private set; }
    public float maxStamina { get; private set; }
    public float attack { get; private set; }
    public float defense { get; private set; }
    public float speed { get; private set; }
    public float money { get; private set; }

    private void Awake()
    {
        Debug.Assert(baseStats != null, 
            "Forgot to drag a scriptable stat container to object: " + gameObject.name);
        if (baseStats != null)
        {
            InitStats();
        }

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                                       "] with root object [" + gameObject.transform.root.name + "] for StatComponent.cs");
            }
        }
    }

    private void Start()
    {
        LocalEventBinding<OnWeaponEquippedEvent> weaponEquippedBinding = new LocalEventBinding<OnWeaponEquippedEvent>(HandleWeaponEquipped);
        localEventHandler.Register(weaponEquippedBinding);
    }

    private void HandleWeaponEquipped(OnWeaponEquippedEvent e)
    {
        // Get difference in stats (between equipped and unequipped) and then modify.
        List<IStatModifier> diffStats = new List<IStatModifier>();

        if (e.unequipped == null)
        {
            ModifyStats(e.equipped.itemStatComponent.statModifiers, true);
        } else
        {
            // Unequip and then equip.
            ModifyStats(e.unequipped.itemStatComponent.statModifiers, false);
            ModifyStats(e.equipped.itemStatComponent.statModifiers, true);
        }
    }

    public void ModifyStat(IStatModifier statModifier)
    {
        switch(statModifier.Stat)
        {
            case Stat.HP:
                health += statModifier.Value;
                break;
            case Stat.MP:
                mana += statModifier.Value;
                break;
            case Stat.SP:
                stamina += statModifier.Value;
                break;
            case Stat.ATK:
                attack += statModifier.Value;
                break;
            case Stat.DEF:
                defense += statModifier.Value;
                break;
            case Stat.SPD:
                speed += statModifier.Value;
                break;
            case Stat.MONEY:
                money += statModifier.Value;
                break;
        }
        OnStatChange?.Invoke(this, statModifier);
    }

    /// <summary>
    /// Modify stats by adding or subtracting from the current stats.
    /// </summary>
    /// <param name="statModifiers"></param>
    /// <param name="isAddition"> when true, stat modifiers are added. when false, stat modifiers are subtractd.</param>
    public void ModifyStats(List<IStatModifier> statModifiers, bool isAddition)
    {
        foreach(IStatModifier statModifier in statModifiers)
        {
            IStatModifier statMod = statModifier;
            if (!isAddition)
            {
                statMod = new IStatModifier(statMod.Stat, -statMod.Value);
            }
            ModifyStat(statMod);
        }
    }

    public float GetCurStat(Stat stat)
    {
        switch(stat)
        {
            case Stat.HP:
                return health;
            case Stat.MP:
                return mana;
            case Stat.SP:
                return stamina;
            case Stat.ATK:
                return attack;
            case Stat.DEF:
                return defense;
            case Stat.SPD:
                return speed;
            case Stat.MONEY:
                return money;
            default:
                return 0;
        }
    }

    public float GetMaxStat(Stat stat)
    {
        switch (stat)
        {
            case Stat.HP:
                return maxHealth;
            case Stat.MP:
                return maxMana;
            case Stat.SP:
                return maxStamina;
            case Stat.ATK:
                return attack;
            case Stat.DEF:
                return defense;
            case Stat.SPD:
                return speed;
            case Stat.MONEY:
                return money;
            default:
                return 0;
        }
    }

    private void InitStats()
    {
        health = baseStats.health;
        maxHealth = baseStats.maxHealth;
        mana = baseStats.mana;
        maxMana = baseStats.maxMana;
        attack = baseStats.attack;
        defense = baseStats.defense;
        speed = baseStats.speed;
        money = 0;
        OnStatChange?.Invoke(this, null);
    }
}
