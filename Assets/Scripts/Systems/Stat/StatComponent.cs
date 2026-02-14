using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Stat Component MonoBehaviour Class that is attached to GameObjects that want to handle stats and events.
/// </summary>
public class StatComponent : MonoBehaviour
{
    // LocalEventHandler only handles event logic between systems on the same object
    [SerializeField] public LocalEventHandler leh;

    public StatContainer StatContainer { get { return statContainer; } private set { } }
    [SerializeField, Required] private StatContainer statContainer;

    // This event allows for subscription between different objects. Eg. UI and StatComponent
    public event Action OnStatChange;

    #region Unity Life Cycle Functions
    private void Awake()
    {
        leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);

        // This binding is made in Awake, since a Call to OnWeaponEquippedEvent happens in Start in EquipmentWeaponHandler
        LocalEventBinding<OnWeaponEquippedEvent> weaponEquippedBinding = new LocalEventBinding<OnWeaponEquippedEvent>(HandleWeaponEquipped);
        leh.Register(weaponEquippedBinding);

        LocalEventBinding<OnDamagedEvent> damageBinding = new LocalEventBinding<OnDamagedEvent>(HandleDamage);
        leh.Register(damageBinding);

        LocalEventBinding<OnStatBuffEvent> buffBinding = new LocalEventBinding<OnStatBuffEvent>(HandleBuff);
        leh.Register(buffBinding);

        LocalEventBinding<OnRespawnEvent> respawnBinding = new LocalEventBinding<OnRespawnEvent>(HandleRespawn);
        leh.Register(respawnBinding);

        Debug.Assert(statContainer != null);
        // If null reference here, most likely statContainer not serialized.
        statContainer.Init();
    }

    private void Start()
    {
        statContainer.StatMediator.RegisterStatChangeListener(HandleStatChange);
    }

    /// <summary>
    /// For debugging purposes.
    /// 
    /// TODO: eventually remove.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //gold += 10;
            HandleDamage(new OnDamagedEvent { damage = 10 });
            Debug.Log($"DEX Stat: {statContainer.Dexterity}");
        }
    }
    #endregion

    #region Local Event Function Handlers
    /// <summary>
    /// Modifies stats based on weapon equipped and weapon unequipped.
    /// </summary>
    /// <param name="e"></param>
    private void HandleWeaponEquipped(OnWeaponEquippedEvent e)
    {
        if (Debug.isDebugBuild) Debug.Log($"Player Atk Stat before handling weapon equipped is [{statContainer.Attack}]");
        Item equipped = e.equipped;
        Item unequipped = e.unequipped;

        if (equipped != null && !equipped.IsEmpty())
        {
            equipped.ItemModifierMediator.OnModifierChange += HandleEquippedWeaponItemChange;

            // Add stat modifiers from equipped item (if equiopped item has stats)
            ApplyWeaponStatModifiers(equipped);
        }

        // Dispose unequipped equipment stat modifiers
        if (unequipped != null && !unequipped.IsEmpty())
        {
            unequipped.ItemModifierMediator.OnModifierChange -= HandleEquippedWeaponItemChange;

            Optional<StatContainer> unequippedStatContainer = unequipped.ItemModifierMediator.GetStatsAfterModification();
            if (unequippedStatContainer.HasValue)
            {
                StatContainer.StatMediator.RemoveModifiersFromSource(unequipped);
            }
            else
            {
                Debug.LogError("Unequipped item doesn't have base stat/proper stat container handling!");
            }
        }

        if (Debug.isDebugBuild)
        {
            //Debug.Log($"Player Atk Stat after handling weapon equipped is [{statContainer.Attack}]");
            //Debug.Log($"Player DEX Stat after handling weapon equipped is [{statContainer.Dexterity}]");
        }
    }
    
    /// <summary>
    /// Modifies health by damage taken
    /// </summary>
    /// <param name="e"></param>
    private void HandleDamage(OnDamagedEvent e)
    {
        statContainer.Health -= e.damage;
        // Invoke Stat Change event
        HandleStatChange();
    }

    /// <summary>
    /// Modifies stat by buff
    /// </summary>
    /// <param name="e"></param>
    private void HandleBuff(OnStatBuffEvent e)
    {
        statContainer.StatMediator.AddModifier(e.buff);
    }

    /// <summary>
    /// Invokes global event for stat change.
    /// </summary>
    /// <param name="e"></param>
    private void HandleStatChange()
    {
        leh.Call(new OnStatChangeEvent { statComponent = this });
        OnStatChange?.Invoke();
    }

    private void HandleRespawn(OnRespawnEvent e)
    {
        statContainer.Health = statContainer.MaxHealth;
        HandleStatChange(); // This notifies BarController to refill the bar
    }
    #endregion    


    /// <summary>
    /// Function that handles whenever equipped weapon is upgraded/changed.
    /// </summary>
    /// <param name="item"></param>
    private void HandleEquippedWeaponItemChange(Item item)
    {
        // Clear previous modifiers.
        StatContainer.StatMediator.RemoveModifiersFromSource(item);

        // Apply the updated one
        // Add stat modifiers from equipped item (if equiopped item has stats)
        ApplyWeaponStatModifiers(item);
    }

    /// <summary>
    /// Helper function that applies the actual stat modifiers from item to entity that owns this StatComponent
    /// </summary>
    /// <param name="item"></param>
    private void ApplyWeaponStatModifiers(Item item)
    {
        Optional<StatContainer> equippedStatContainer = item.ItemModifierMediator.QueryStatsAfterModification();

        if (equippedStatContainer.HasValue)
        {
            StatContainer esc = equippedStatContainer.Value;
            StatModifier equippedATKStatModifier = new StatModifier(Stat.ATK, new AddOperation(esc.Attack), -1);
            StatModifier equippedDEXStatModifier = new StatModifier(Stat.DEX, new AddOperation(esc.Dexterity), -1);
            StatModifier equippedDEFStatModifier = new StatModifier(Stat.DEF, new AddOperation(esc.Defense), -1);
            StatModifier equippedSPDStatModifier = new StatModifier(Stat.SPD, new AddOperation(esc.Speed), -1);

            StatContainer.StatMediator.AddModifier(item, equippedATKStatModifier);
            StatContainer.StatMediator.AddModifier(item, equippedDEXStatModifier);
            StatContainer.StatMediator.AddModifier(item, equippedDEFStatModifier);
            StatContainer.StatMediator.AddModifier(item, equippedSPDStatModifier);

            if (Debug.isDebugBuild)
            {
                //Debug.Log($"Equipped weapon ATK value is: " + esc.Attack);
                //Debug.Log($"Equipped weapon DEX value is: " + esc.Dexterity);
                //Debug.Log($"Equipped weapon DEF value is: " + esc.Defense);
                //Debug.Log($"Equipped weapon SPD value is: " + esc.Speed);
            }
        }
        else
        {
            Debug.LogError("Equipped item doesn't have base stat/proper stat container handling!");
        }
    }

}
