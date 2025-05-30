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

        if (equipped != null)
        {
            equipped.ItemModifierMediator.OnModifierChange += HandleEquippedWeaponItemChange;

            // Add stat modifiers from equipped item (if equiopped item has stats)
            Optional<StatContainer> equippedStatContainer = equipped.ItemModifierMediator.GetStatsAfterModification();

            if (equippedStatContainer.HasValue)
            {
                StatContainer esc = equippedStatContainer.Value;
                StatModifier equippedStatModifier = new StatModifier(Stat.ATK, new AddOperation(esc.Attack), -1);
                StatContainer.StatMediator.AddModifier(equipped, equippedStatModifier);

                if (Debug.isDebugBuild) Debug.Log($"Equipped weapon atk value is: " + esc.Attack);
            } else
            {
                Debug.LogError("Equipped item doesn't have base stat/proper stat container handling!");
            }
        }

        // Dispose unequipped equipment stat modifiers
        if (unequipped != null)
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

        if (Debug.isDebugBuild) Debug.Log($"Player Atk Stat after handling weapon equipped is [{statContainer.Attack}]");
    }
    
    /// <summary>
    /// Modifies health by damage taken
    /// </summary>
    /// <param name="e"></param>
    private void HandleDamage(OnDamagedEvent e)
    {
        StatModifier damageModifier = new StatModifier(Stat.HP, new AddOperation(-e.damage), -1);
        statContainer.StatMediator.AddModifier(damageModifier);
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
    #endregion    

    private void HandleEquippedWeaponItemChange(Item item)
    {
        // Clear previous modifiers.
        StatContainer.StatMediator.RemoveModifiersFromSource(item);

        // Apply the updated one
        // Add stat modifiers from equipped item (if equiopped item has stats)
        Optional<StatContainer> equippedStatContainer = item.ItemModifierMediator.GetStatsAfterModification();

        if (equippedStatContainer.HasValue)
        {
            StatContainer esc = equippedStatContainer.Value;
            StatModifier equippedStatModifier = new StatModifier(Stat.ATK, new AddOperation(esc.Attack), -1);
            StatContainer.StatMediator.AddModifier(item, equippedStatModifier);

            if (Debug.isDebugBuild) Debug.Log($"Equipped weapon atk value is: " + esc.Attack);
        }
        else
        {
            Debug.LogError("Equipped item doesn't have base stat/proper stat container handling!");
        }
    }
}
