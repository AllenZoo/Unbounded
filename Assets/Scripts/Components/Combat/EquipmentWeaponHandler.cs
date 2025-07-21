using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Attach to Attack Handler (with Attacker)
// Handles modifying the attacker object with item equipped.
[RequireComponent(typeof(AttackerComponent))]
public class EquipmentWeaponHandler : MonoBehaviour, IDataPersistence
{
    [Required, SerializeField] private LocalEventHandler leh;

    [Tooltip("Inventory we will look for weapon item for in.")]
    [Required, SerializeField] private InventorySystemContext inventorySystemContext;
    [SerializeField] private int weaponSlotIndex = 0;
    [SerializeField] private AttackerComponent attackerComponent;

    private InventorySystem inventory;
    [SerializeField, ReadOnly] private Item curWeapon;
    [SerializeField, ReadOnly] private Item previousWeapon;

    private void Awake()
    {
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


        EventBinding<OnStarterWeaponCardApplyEffect> oswcaeBinding = new EventBinding<OnStarterWeaponCardApplyEffect>(HandleOnStarterWeaponPickedEvent);
        EventBus<OnStarterWeaponCardApplyEffect>.Register(oswcaeBinding);
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
        curWeapon = item.IsEmpty() ? null : item;

        // Don't update anything if curWeapon and previousWeapon are the same.
        if (CheckItemEqual(curWeapon, previousWeapon)) return;

        Debug.Log("Updating Attacker (should only see this once)");
        UpdateAttacker(curWeapon);

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
        if (attackerItem == null)
        {
            attackerComponent.SetAttacker(null);
            return;
        }

        if (attackerItem.ItemModifierMediator == null)
        {
            Debug.Log($"ItemModiferMediator is null? {attackerItem.ItemModifierMediator == null}");
        }
       
        Attacker attackerToSet = attackerItem?.ItemModifierMediator?.GetAttackerAfterModification();
        attackerComponent.SetAttacker(attackerToSet);
    }

    private void UpdateAttacker()
    {
        UpdateAttacker(curWeapon);
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

    private void HandleOnStarterWeaponPickedEvent (OnStarterWeaponCardApplyEffect e)
    {
        if (!inventory.GetItem(0).IsEmpty())
        {
            // TODO: consider blocking new weapon equip. Game logic shouldn't warrant being able to do this anyways.

            Debug.LogError("Warning! We already have an equipped weapon to player. Overriding old weapon!");
            //Debug.LogError("Selected Starter Weapon when we already have a weapon equipped to player!");
            //return;
        }

        // Equip that item into inventory system
        Item newWeapon = e.cardData.Item.Clone();
        newWeapon.Init();
        inventory.SetItem(newWeapon, 0);
        UpdateWeapon();
    }

    private bool CheckItemEqual(Item item1, Item item2)
    {
        if (item1 == null)
        {
            return item2 == null;
        }

        if(item2 == null)
        {
            return false;
        }

        return item1.Equals(item2);
    }

    public void LoadData(GameData data)
    {
        //throw new NotImplementedException();
        Debug.Log("Loading weapon inside equipment weapon handler!");
        // TODO: move this save load logic to inventory.

        curWeapon = data.playerEquippedWeapon;
        UpdateAttacker();
    }

    public void SaveData(GameData data)
    {
        Debug.Log("Saving inside equipment weapon handler!");
        data.playerEquippedWeapon = curWeapon;
        //throw new NotImplementedException();
    }
}
