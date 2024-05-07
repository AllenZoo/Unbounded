using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatComponent : MonoBehaviour
{
    [SerializeField] private LocalEventHandler localEventHandler;

    // TODO: eventually remove this.
    public event Action<StatComponent, StatModifier> OnStatChange;


    [SerializeField] private SO_StatContainer baseStats;

    private StatMediator statMediator;

    public float health
    {
        get
        {
            var q = new StatQuery(Stat.HP, baseStats.health);
            statMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float maxHealth
    {
        get {
            var q = new StatQuery(Stat.MAX_HP, baseStats.maxHealth);
            statMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float mana
    {
        get
        {
            var q = new StatQuery(Stat.MP, baseStats.mana);
            statMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float maxMana
    {
        get
        {
            var q = new StatQuery(Stat.MAX_MP, baseStats.maxMana);
            statMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float stamina
    {
        get
        {
            var q = new StatQuery(Stat.SP, baseStats.stamina);
            statMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float maxStamina
    {
        get
        {
            var q = new StatQuery(Stat.MAX_SP, baseStats.maxStamina);
            statMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float attack {
        get {
            var q = new StatQuery(Stat.ATK, baseStats.attack);
            statMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { } 
    }
    public float defense {
        get {
            var q = new StatQuery(Stat.DEF, baseStats.defense);
            statMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float speed {
        get {
            var q = new StatQuery(Stat.SPD, baseStats.speed);
            statMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }
    public float gold {
        get {
            var q = new StatQuery(Stat.GOLD, baseStats.gold);
            statMediator.CalculateFinalStat(q);
            return q.Value;
        }
        private set { }
    }

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
        statMediator = new StatMediator(localEventHandler);

        // This binding is made in Awake, since a Call to OnWeaponEquippedEvent happens in Start in EquipmentWeaponHandler
        LocalEventBinding<OnWeaponEquippedEvent> weaponEquippedBinding = new LocalEventBinding<OnWeaponEquippedEvent>(HandleWeaponEquipped);
        localEventHandler.Register(weaponEquippedBinding);
    }

    private void Start()
    {
        

    }

    public float GetStatValue(Stat stat)
    {
        switch(stat)
        {
            case Stat.HP:
                return health;
            case Stat.MAX_HP:
                return maxHealth;
            case Stat.MP:
                return mana;
            case Stat.MAX_MP:
                return maxMana;
            case Stat.SP:
                return stamina;
            case Stat.MAX_SP:
                return maxStamina;
            case Stat.ATK:
                return attack;
            case Stat.DEF:
                return defense;
            case Stat.SPD:
                return speed;
            case Stat.GOLD:
                return gold;
            default:
                return 0;
        }
    }

    private void HandleWeaponEquipped(OnWeaponEquippedEvent e)
    {
        if (e.equipped != null)
        {
            // Add stat modifiers from equipped weapon
            e.equipped.itemStatComponent.statModifiers.ForEach((statModifier) => {
                statMediator.AddModifier(statModifier.GetModifier());
            });

            e.equipped.itemUpgradeComponent.upgradeStatModifiers.ForEach((statModifier) =>
            {
                statMediator.AddModifier(statModifier.GetModifier());
            });
        }
        
        // Dispose unequipped equipment stat modifiers
        if (e.unequipped != null)
        {
            e.unequipped.itemStatComponent.statModifiers.ForEach((statModifier) =>
            {
                statMediator.RemoveModifier(statModifier.GetModifier());
            });
            e.unequipped.itemUpgradeComponent.upgradeStatModifiers.ForEach((statModifier) =>
            {
                statMediator.RemoveModifier(statModifier.GetModifier());
            });
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
        gold = 0;
        OnStatChange?.Invoke(this, null);
    }

    private void Update()
    {
        // For Debugging
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var q = new StatQuery(Stat.ATK, baseStats.attack);
            statMediator.CalculateFinalStat(q);
            Debug.Log(gameObject.name + " Attack: " + q.Value);
            // statMediator.AddModifier(new StatModifier(Stat.SPD, new AddOperation(10), -1));
        }
    }
}
