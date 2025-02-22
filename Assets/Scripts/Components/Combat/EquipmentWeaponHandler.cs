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
    [SerializeField] private LocalEventHandler localEventHandler;

    // TODO: decide whether to have reference to SO_Inventory or InventorySystem.
    // Probably better to have ref to SO_Inventory.
    [Tooltip("Equipment inventory")]
    [SerializeField] private InventorySystem inventory;
    [SerializeField] private int weaponSlotIndex;

    [Tooltip("Stat component to modify when equipping weapon items.")]
    [SerializeField] private StatComponent stat;

    // TODO: REFACTOR!! Consider adding some SetAttacker to AttackerComponent
    private Attacker attacker;
    private Item previousWeapon;

    private void Awake()
    {
        // Assert.IsNotNull(inventory, "EquipmentWeaponHandler needs inventory.");
        Assert.IsNotNull(stat, "EquipmentWeaponHandler needs stat component to modify.");
        attacker = GetComponent<Attacker>();

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                    "] with root object [" + gameObject.transform.root.name + "] for EquipmentWeaponHandler.cs");
            }
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

    private void UpdateWeapon()
    {
        // Get item from inventory.
        Item item = inventory.GetItem(weaponSlotIndex);

        // If item is null, then we don't have a weapon equipped.
        if (item == null || item.IsEmpty())
        {
            localEventHandler.Call(new OnWeaponEquippedEvent { equipped = null, unequipped = previousWeapon });
            previousWeapon = null;
            throw new NotImplementedException("TODO: reimplement setting attacker data");
            // attacker.SetAttackerData((AttackerData) null);
            return;
        }

        // If item doesn't contain an ItemAttackContainerComponent, then throw an error since this shouldn't happen.
        if (!item.HasComponent<ItemAttackContainerComponent>())
        {
            Debug.LogError("ERROR: Item in weapon slot does not contain an attack container component!");
            return;
        }

        // Set attacker data to the attack data in the item.
        ItemAttackContainerComponent attackComponent = item.GetComponent<ItemAttackContainerComponent>();
        if (attackComponent != null && attackComponent.attackerData != null)
        {
            throw new NotImplementedException("TODO: reimplement setting attacker data");
            // attacker.SetAttackerData(attackComponent.attackerData);
        } else
        {
            Debug.LogError("ERROR: ItemAttackContainerComponent doesn't contain a proper AttackerData!");
        }

        localEventHandler.Call(new OnWeaponEquippedEvent { equipped = item, unequipped = previousWeapon });
        previousWeapon = item;
    }

}
