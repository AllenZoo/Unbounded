using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by the Upgrade/Forging System.
/// </summary>
public class ForgerSystem : MonoBehaviour
{
    // TODO: maybe eventually think about some event: OnForgeRequest { playerMoney: int, equipment: Item, stones: List<Item> }
    [Required]
    [SerializeField] private SO_Inventory upgradeInventory;

    [Required]
    [SerializeField] private SO_Inventory equipmentToForgeInventory;

    [Required]
    [SerializeField] private SO_Inventory previewInventory;

    [Required]
    [Tooltip("Used for enabling/disabling interactivity with the preview item.")]
    [SerializeField] private InventoryUI previewInventoryUI;


    private IForger forger;

    // Used to persist the preview item when the preview item after forging.
    private bool persistPreviewItem = false;

    private bool canForge = false;

    private void Awake()
    {
        forger = new Forger();

        upgradeInventory.OnInventoryDataChange += UpdatePreview;
        equipmentToForgeInventory.OnInventoryDataChange += UpdatePreview;
        previewInventory.OnInventoryDataChange += OnPreviewInventoryChange;
    }

    private void Start()
    {
        // upgradeInventory = InventorySystemStorage.Instance.GetSystem(InventoryType.)
    }

    /// <summary>
    /// Forges the equipment using the stones in the upgrade inventory. Consumes all stones in the upgrade inventory.
    /// </summary>
    public void Forge()
    {
        UpdatePreviewItem();
        // Check forge conditions before consuming items.
        if (!canForge)
        {
            Debug.Log("Attempted to forge without satisfying conditions.");
            return;
        }

        Item curPreviewItem = previewInventory.items[0];
        if (curPreviewItem != null && persistPreviewItem)
        {
            // Cannot forge if previous item has not be added to inventory.
            return;
        }

        persistPreviewItem = true;

        // Spend gold from player (important to do this before consuming stones)
        PlayerSingleton.Instance.GetComponentInChildren<StatComponent>().StatContainer.Gold -= GetForgeCost();

        // Consume stones
        upgradeInventory.ClearInventory();

        // Destroy equipment pre-forge
        equipmentToForgeInventory.Set(0, null);

        // Enable interactivity with the preview item.
        previewInventoryUI.EnableSlot(0);
    }

    /// <summary>
    /// Creates the preview item and puts it in the preview inventory.
    /// </summary>
    private void UpdatePreviewItem()
    {
        // If we are persisting the preview item, we don't want to update it.
        if (persistPreviewItem)
        {
            return;
        }

        // Components involved in forging
        List<Item> stones = upgradeInventory.items;
        Item equipment = equipmentToForgeInventory.items[0];

        //Check if we can forge
        // TODO-OPT: We can disable the forge button if we can't forge.
        if (!CheckForgeConditions(stones, equipment))
        {
            Debug.Log("Cannot forge weapon. Did not satisfy conditions!");
            previewInventory.Set(0, null);
            canForge = false;
            return;
        }

        canForge = true;

        // Forge preview equipment
        Item forgedEquipment = forger.Forge(stones, equipment);

        // Insert weapon into preview inventory.
        previewInventory.Set(0, forgedEquipment);

        // Disable interactivity with the preview item.
        previewInventoryUI.DisableSlot(0);
    }


    /// <summary>
    /// Returns true if we can forge.
    /// Checks: 
    ///     1. If player has enough money.
    ///     2. If equipment is not null.
    ///     3. If stones are not empty.
    ///     4. If upgraders have upgrader item components, and equipment has upgrade item components.
    /// </summary>
    /// <returns></returns>
    private bool CheckForgeConditions(List<Item> stones, Item equipment)
    {
        foreach (Item item in stones)
        {
            if (item == null || item.IsEmpty()) continue;

            if (!item.HasComponent<ItemUpgraderComponent>()) return false;
        }
        float cost = GetForgeCost(stones);
        bool equipmentNotNull = equipment != null && !equipment.IsEmpty();
        return equipmentNotNull && equipment.HasComponent<ItemUpgradeComponent>() && CheckFunds(cost) && CheckInventories();
    }

    /// <summary>
    /// Calculates the total cost of forging stones onto the equipment
    /// </summary>
    /// <param name="stones"></param>
    /// <returns></returns>
    public float GetForgeCost(List<Item> stones)
    {
        float cost = 0;
        foreach (Item item in stones)
        {
            if (item == null || item.IsEmpty())
            {
                continue;
            }

            float goldNeededPerItem = item.GetComponent<ItemUpgraderComponent>().costPerItem;
            cost += goldNeededPerItem * item.quantity;
        }

        return cost;
    }

    public float GetForgeCost()
    {
        return GetForgeCost(upgradeInventory.items);
    }
    
    /// <summary>
    /// Returns the money of player after forging.
    /// </summary>
    /// <returns></returns>
    public float GetAfterForgeResult()
    {
        return GetCurMoney() - GetForgeCost();
    }

    public float GetCurMoney()
    {
        return PlayerSingleton.Instance.GetComponentInChildren<StatComponent>().StatContainer.Gold;
    }

    /// <summary>
    /// Checks if the player has enough funds to forge the equipment.
    /// </summary>
    /// <returns>true if player has enough money.</returns>
    private bool CheckFunds(float cost)
    {
        float curMoney = PlayerSingleton.Instance.GetComponentInChildren<StatComponent>().StatContainer.Gold;
        return curMoney >= cost;
    }
    
    /// <summary>
    /// Checks if the upgrade inventory and equipment to forge inventory are not empty.
    /// </summary>
    /// <returns></returns>
    private bool CheckInventories()
    {
        return !upgradeInventory.IsEmpty() && !equipmentToForgeInventory.IsEmpty();
    }

    private void UpdatePreview()
    {
        // Get forge item preview and insert preview item into preview inventory.
        // Don't consume stones.
        UpdatePreviewItem();
    }

    private void OnPreviewInventoryChange()
    {
        if (previewInventory.IsEmpty())
        {
            persistPreviewItem = false;
        }
        
    }
}
