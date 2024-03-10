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
    [SerializeField] private InventorySystem inventory;
    [SerializeField] private int weaponSlotIndex;
    [Tooltip("Stat component to modify when equipping weapon items.")]
    [SerializeField] private StatComponent stat;
    private Attacker attacker;
    private List<IStatModifier> curAppliedStats;

    private void Awake()
    {
        // Assert.IsNotNull(inventory, "EquipmentWeaponHandler needs inventory.");
        Assert.IsNotNull(stat, "EquipmentWeaponHandler needs stat component to modify.");
        attacker = GetComponent<Attacker>();
        curAppliedStats = new List<IStatModifier>();
    }

    private void Start()
    {
        inventory = InventorySystemStorage.Instance.GetSystem(InventoryType.Equipment);
        inventory.OnInventoryDataModified += UpdateWeapon;
        UpdateWeapon();
    }

    // TODO: refactor SO_Weapon_Item to store SO_Attacker instead of attackObj.
    private void UpdateWeapon()
    {
        // Get weapon from inventory.
        Item weapon = inventory.GetItem(weaponSlotIndex);
        // If weapon is null, then we don't have a weapon equipped.
        if (weapon == null || weapon.data == null)
        {
            Debug.Log("No weapon found in slot " + weaponSlotIndex + ".");
            // Set attack object to null.
            attacker.SetAttacker(null);
            return;
        }
        // Set attack object to weapon's attack object.
        // attacker.SetAttackObj(weapon.attacker.data.attackObj);
        attacker.SetAttacker(weapon.data.attacker);

        // Clear all stat current stat modifiers.
        ClearStatModifiers();

        // Add all stat modifiers from weapon.
        foreach (IStatModifier statMod in weapon.data.statModifiers)
        {
            curAppliedStats.Add(statMod);
        }

        // Apply all stat modifiers.
        ApplyStatModifiers();
    }

    private void ClearStatModifiers()
    {
        if (curAppliedStats != null)
        {
            // Remove all applied stat modifiers.
            foreach (IStatModifier statMod in curAppliedStats)
            {
                IStatModifier clearStat = new IStatModifier(statMod.Stat, -statMod.Value);
                stat.ModifyStat(clearStat);
            }
        }
        curAppliedStats.Clear();
    }

    private void ApplyStatModifiers()
    {
        if (curAppliedStats != null)
        {
            // Remove all applied stat modifiers.
            foreach (IStatModifier statMod in curAppliedStats)
            {
                stat.ModifyStat(statMod);
            }
        }
    }


}
