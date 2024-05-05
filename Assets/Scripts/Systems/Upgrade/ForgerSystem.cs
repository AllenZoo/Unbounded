using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used by the Upgrade/Forging System.
/// </summary>
public class ForgerSystem : MonoBehaviour
{
    // TODO: maybe eventually think about some event: OnForgeRequest { playerMoney: int, equipment: Item, stones: List<Item> }
    [SerializeField] private SO_Inventory upgradeInventory;
    [SerializeField] private SO_Inventory equipmentToForgeInventory;
    [SerializeField] private SO_Inventory previewInventory;

    private IForger forger;

    private void Awake()
    {
        forger = new Forger();
    }

    /// <summary>
    /// Forges the equipment using the stones in the upgrade inventory. Consumes all stones in the upgrade inventory.
    /// </summary>
    public void Forge()
    {
        // Components involved in forging
        List<Item> stones = upgradeInventory.items;
        Item equipment = equipmentToForgeInventory.items[0];

        //Check if we can forge
        if (!CheckForgeConditions(stones, equipment))
        {
            Debug.Log("Cannot forge weapon. Did not satisfy conditions!");
            return;
        }

        // Forge equipment
        Item forgedEquipment = forger.Forge(stones, equipment);

        // Insert weapon into preview inventory.
        previewInventory.items[0] = forgedEquipment;

        // Consume stones
        upgradeInventory.items.Clear();
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

            foreach (IItemComponent itemComp in item.data.GetItemComponents())
            {
                if (itemComp is ItemValueComponent)
                {
                    ItemValueComponent itemValueComponent = itemComp as ItemValueComponent;
                    cost += itemValueComponent.goldValue * item.quantity;
                }
            }
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
        return PlayerSingleton.Instance.GetComponentInChildren<StatComponent>().GetCurStat(Stat.MONEY);
    }

    /// <summary>
    /// Returns true if we can forge.
    /// </summary>
    /// <returns></returns>
    private bool CheckForgeConditions(List<Item> stones, Item equipment)
    {
        float cost = GetForgeCost(stones);
        return CheckFunds(cost) && CheckInventories();
    }

    /// <summary>
    /// Checks if the player has enough funds to forge the equipment.
    /// </summary>
    /// <returns>true if player has enough money.</returns>
    private bool CheckFunds(float cost)
    {
        float curMoney = PlayerSingleton.Instance.GetComponentInChildren<StatComponent>().GetCurStat(Stat.MONEY);
        return curMoney >= cost;
    }
    
    private bool CheckInventories()
    {
        return !upgradeInventory.IsEmpty() && !equipmentToForgeInventory.IsEmpty();
    }


}
