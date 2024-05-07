using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Attach to Attack Handler (with Attacker)
// Handles modifying the attacker object with item equipped.
[RequireComponent(typeof(Attacker))]
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
    
    private Attacker attacker;
    private SO_Weapon_Item previousWeapon;

    private List<StatModifier> curAppliedStats;

    private void Awake()
    {
        // Assert.IsNotNull(inventory, "EquipmentWeaponHandler needs inventory.");
        Assert.IsNotNull(stat, "EquipmentWeaponHandler needs stat component to modify.");
        attacker = GetComponent<Attacker>();
        curAppliedStats = new List<StatModifier>();

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

    // TODO: refactor SO_Weapon_Item to store SO_Attacker instead of attackObj.
    private void UpdateWeapon()
    {
        // Get item from inventory.
        Item item = inventory.GetItem(weaponSlotIndex);

        // If item is null, then we don't have a weapon equipped.
        if (item == null || item.IsEmpty())
        {
            localEventHandler.Call(new OnWeaponEquippedEvent { equipped = null, unequipped = previousWeapon });
            previousWeapon = null;
            attacker.SetAttackerData((AttackerData) null);
            return;
        }

        // Check if item is an item of type weapon. (if not null)
        if (item != null && !(item.data is SO_Weapon_Item w))
        {
            // It's not. (This should never happen)
            Debug.LogError("Item in weapon slot is not a weapon item!");
        }

        SO_Weapon_Item weapon = item.data as SO_Weapon_Item;
        attacker.SetAttackerData(weapon.itemAttackComponent.attackerData);

        localEventHandler.Call(new OnWeaponEquippedEvent { equipped = weapon, unequipped = previousWeapon });
        previousWeapon = weapon;
    }

}
