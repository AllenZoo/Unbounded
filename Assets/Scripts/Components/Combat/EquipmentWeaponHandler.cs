using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Attach to Attack Handler (with Attacker)
// Handles modifying the attacker object with item equipped.
[RequireComponent(typeof(Attacker))]
public class EquipmentWeaponHandler : MonoBehaviour
{
    // TODO: decide whether to have reference to SO_Inventory or InventorySystem.
    [Tooltip("Equipment inventory")]
    [SerializeField] private SO_Inventory inventory;
    [SerializeField] private int weaponSlotIndex;
    private Attacker attacker;

    private void Awake()
    {
        Assert.IsNotNull(inventory, "EquipmentWeaponHandler needs inventory");
        attacker = GetComponent<Attacker>();
    }

    private void Start()
    {
        inventory.OnInventoryDataChange += UpdateWeapon;
        UpdateWeapon();
    }

    private void UpdateWeapon()
    {
        // Get weapon from inventory.
        SO_Item weapon = inventory.items[weaponSlotIndex];
        // If weapon is null, then we don't have a weapon equipped.
        if (weapon == null)
        {
            Debug.Log("No weapon found in slot " + weaponSlotIndex + ".");
            // Set attack object to null.
            attacker.SetAttackObj(null);
            return;
        }
        // Set attack object to weapon's attack object.
        attacker.SetAttackObj(weapon.weaponItem.attackObj);
    }


}
