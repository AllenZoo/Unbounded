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
    [Required]
    [SerializeField] 
    private LocalEventHandler localEventHandler;

    // TODO: decide whether to have reference to SO_Inventory or InventorySystem.
    // Probably better to have ref to SO_Inventory.
    [Tooltip("Equipment inventory")]
    [SerializeField] private InventorySystem inventory;
    [SerializeField] private int weaponSlotIndex;

    [Tooltip("Stat component to modify when equipping weapon items.")]
    [SerializeField] private StatComponent stat;

    private AttackerComponent attackerComponent;
    private Item previousWeapon;

    private void Awake()
    {
        // Assert.IsNotNull(inventory, "EquipmentWeaponHandler needs inventory.");
        Assert.IsNotNull(stat, "EquipmentWeaponHandler needs stat component to modify.");
        attackerComponent = GetComponent<AttackerComponent>();

        if (localEventHandler == null)
        {
            localEventHandler = InitializerUtil.FindComponentInParent<LocalEventHandler>(this.gameObject);
        }
    }

    private void Start()
    {
        inventory = InventorySystemStorage.Instance.GetSystem(InventoryType.Equipment);

        if (inventory != null)
        {
            inventory.OnInventoryDataModified += UpdateWeapon;
            UpdateWeapon();
        }
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
            localEventHandler.Call(new OnWeaponEquippedEvent { equipped = null, unequipped = previousWeapon });
            previousWeapon = null;
            attackerComponent.SetAttacker(null);
            return;
        }

        // throw new NotImplementedException();

        // TODO: we store attacker data in item itself and not ItemAttackContainerComponent. Handle trait modification here, using ItemModifierMediator to return dynamically
        //       modified attacker.
        // If item doesn't contain an ItemAttackContainerComponent, then throw an error since this shouldn't happen.
        Attacker attackerToSet = item.data.attacker;
        if (attackerToSet == null)
        {
            Debug.LogError("ERROR: Item in weapon slot does not contain an attack component! Did we equip an item that cannot attack?");
            return;
        }

        attackerComponent.SetAttacker(attackerToSet);
        localEventHandler.Call(new OnWeaponEquippedEvent { equipped = item, unequipped = previousWeapon });
        previousWeapon = item;
    }
}
