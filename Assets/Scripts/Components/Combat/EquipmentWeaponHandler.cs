using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Attach to Attack Handler (with Attacker)
// Handles modifying the attacker object with item equipped.
[RequireComponent(typeof(AttackerComponent))]
public class EquipmentWeaponHandler : MonoBehaviour
{
    [Required, SerializeField] private LocalEventHandler leh;

    // TODO: decide whether to have reference to SO_Inventory or InventorySystem.
    // Probably better to have ref to SO_Inventory.
    [Tooltip("Equipment inventory")]
    [SerializeField] private InventorySystem inventory;
    [SerializeField] private int weaponSlotIndex = 0;
    [SerializeField] private AttackerComponent attackerComponent;

    private Item curWeapon;
    private Item previousWeapon;

    private void Awake()
    {
        // Assert.IsNotNull(inventory, "EquipmentWeaponHandler needs inventory.");
        attackerComponent = GetComponent<AttackerComponent>();

        if (leh == null) leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(this.gameObject);

    }

    private void Start()
    {
        inventory = InventorySystemStorage.Instance.GetSystem(InventoryType.Equipment);

        if (inventory != null)
        {
            inventory.OnInventoryDataModified += UpdateWeapon;
            UpdateWeapon();
        }

        EventBinding<OnUpgradeCardApplyEffect> ucaeBinding = new EventBinding<OnUpgradeCardApplyEffect>(HandleOnUCAEBindingEvent);
        EventBus<OnUpgradeCardApplyEffect>.Register(ucaeBinding);
    }

    /// <summary>
    /// Gets weapon from inventory and then updates the corresponding Attacker Component reference to use 
    /// the attack behaviour specified by item.
    /// </summary>
    private void UpdateWeapon()
    {
        // Get item from inventory.
        Item item = inventory.GetItem(weaponSlotIndex);

        // If item is null, then we don't have a weapon equipped.
        // Note: isEmpty() checks if item.data is null.
        if (item == null || item.IsEmpty())
        {
            leh.Call(new OnWeaponEquippedEvent { equipped = null, unequipped = previousWeapon });
            previousWeapon = null;
            attackerComponent.SetAttacker(null);
            return;
        }

        Attacker attackerToSet = item.ItemModifierMediator.GetAttackerAfterModification();
        if (attackerToSet == null)
        {
            Debug.LogError("ERROR: Item in weapon slot does not contain an attack component! Did we equip an item that cannot attack?");
            return;
        }
        attackerComponent.SetAttacker(attackerToSet);

        previousWeapon = curWeapon;
        curWeapon = item;
        leh.Call(new OnWeaponEquippedEvent { equipped = curWeapon, unequipped = previousWeapon });
    }

    private void HandleOnUCAEBindingEvent(OnUpgradeCardApplyEffect e)
    {
        if (curWeapon == null) {
            Debug.LogError("Upgrade Card applied to null weapon! Shouldn't ever happen if we design game properly.");
            return;
        }

        curWeapon.GetComponent<ItemUpgradeComponent>().AddCard(e.cardData);
    }
}
