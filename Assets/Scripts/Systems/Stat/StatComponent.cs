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

    #region Local Event Function Handlers
    /// <summary>
    /// Modifies stats based on weapon equipped and weapon unequipped.
    /// </summary>
    /// <param name="e"></param>
    private void HandleWeaponEquipped(OnWeaponEquippedEvent e)
    {
        Item equipped = e.equipped;
        Item unequipped = e.unequipped;

        if (equipped != null)
        {
            throw new NotImplementedException();
            // Add stat modifiers from equipped weapon
            //if (equipped.HasComponent<ItemBaseStatComponent>())
            //{
            //    equipped.GetComponent<ItemBaseStatComponent>().statModifiers.ForEach((statModifier) =>
            //    {
            //        statMediator.AddModifier(statModifier.GetModifier());
            //    });
            //}

            //if (equipped.HasComponent<ItemUpgradeComponent>())
            //{
            //    equipped.GetComponent<ItemUpgradeComponent>().upgradeStatModifiers.ForEach((statModifier) =>
            //    {
            //        statMediator.AddModifier(statModifier.GetModifier());
            //    });
            //}
        }

        // Dispose unequipped equipment stat modifiers
        if (unequipped != null)
        {
            throw new NotImplementedException();
            //if (unequipped.HasComponent<ItemBaseStatComponent>())
            // {
            //     unequipped.GetComponent<ItemBaseStatComponent>().statModifiers.ForEach((statModifier) =>
            //     {
            //         statMediator.RemoveModifier(statModifier.GetModifier());
            //     });
            // }
            //if (unequipped.HasComponent<ItemUpgradeComponent>())
            //{
            //    unequipped.GetComponent<ItemUpgradeComponent>().upgradeStatModifiers.ForEach((statModifier) =>
            //    {
            //        statMediator.RemoveModifier(statModifier.GetModifier());
            //    });
            //}
        }
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
}
