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

    [Tooltip("Inventory we will look for weapon item for in.")]
    [Required, SerializeField] private InventorySystemContext inventorySystemContext;
    [SerializeField] private int weaponSlotIndex = 0;
    [SerializeField] private AttackerComponent attackerComponent;

    private InventorySystem inventory;
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
        inventory = inventorySystemContext.Get();

        if (inventory != null)
        {
            inventory.OnInventoryDataModified += UpdateWeapon;
            UpdateWeapon();
        } else
        {
            Debug.LogError("Inventory System not given to equipment weapon handler!");
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
        // Food for thought:
        //   *This function is called everytime the Equipment inventory is modified.*
        //   Since we only have 1 weapon slot, there shouldn't be any bugs where we somehow equip and unequip the same weapon.
        //   However keep this in mind, for finding any potential bugs in the future.

        // Get item from inventory.
        Item item = inventory.GetItem(weaponSlotIndex);
        previousWeapon = curWeapon;
        curWeapon = item;

        UpdateAttacker(item);

        if (previousWeapon?.ItemModifierMediator != null)
        {
            previousWeapon.ItemModifierMediator.OnModifierChange -= UpdateAttacker;
        }
        if (curWeapon?.ItemModifierMediator != null)
        {
            curWeapon.ItemModifierMediator.OnModifierChange += UpdateAttacker;
        }


        leh.Call(new OnWeaponEquippedEvent { equipped = curWeapon, unequipped = previousWeapon });
    }

    /// <summary>
    /// Sets the attacker given item.
    /// Note: Added as a listener to ItemModfierMediator.OnModifierChange so that we reflect the attacker changes too.
    /// </summary>
    /// <param name="attackerItem"></param>
    private void UpdateAttacker(Item attackerItem)
    {
        Attacker attackerToSet = attackerItem?.ItemModifierMediator.GetAttackerAfterModification();
        attackerComponent.SetAttacker(attackerToSet);
    }

    /// <summary>
    /// Handles UCAE = UpgradeCardApplyEffect event.
    /// </summary>
    /// <param name="e"></param>
    private void HandleOnUCAEBindingEvent(OnUpgradeCardApplyEffect e)
    {
        if (curWeapon == null) {
            Debug.LogError("Upgrade Card applied to null weapon! Shouldn't ever happen if we design game properly.");
            return;
        }

        curWeapon.GetComponent<ItemUpgradeComponent>().AddCard(e.cardData);
    }
}
