using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Manages the inventory data. Does not handle UI, but processes requests to add/remove/swap items.
public class InventorySystem : MonoBehaviour
{
    // Refers to when items in inventory are added/removed/swapped.
    public event Action OnInventoryDataModified;

    // Refers to when inventory data is reset, or changed to a different seperate
    // SO_Inventory.
    public event Action<SO_Inventory> OnInventoryDataReset;

    // [Header("Init through SO or manual. If SO takes precendence.")]
    [Tooltip("Inventory Data to be used")]
    [SerializeField] private SO_Inventory inventoryData;

    //[Tooltip("Inventory Data to be used")]
    //[SerializeField] private List<SO_Item> items;
    //[SerializeField] private float numSlots = 9;

    // Maps each slot and their respective rules.
    // Implement interface ConditionMet(SO_Item item) for each condition
    // to be met. If index of slot not in dictionary, then no rules for that slot.
    // NOTE: SerializedDictionary cannot serialize lists of interfaces as values, so we use
    // enums to represent each condition.
    // NOTE: also cant serialize list of enums, so we use SO_Conditions to store the enums.
    [Tooltip("For adding specific rules for specified slots.")]
    [SerializedDictionary("Slot index", "Slot conditions")]
    [SerializeField] private SerializedDictionary<int, SO_Conditions> slotRules;

    // Ref to inventory data.
    
    private Inventory inventory;

    private void Awake()
    {
        // Check that inventoryData is not null
        Assert.IsNotNull(inventoryData);

        if (slotRules == null)
        {
            slotRules = new SerializedDictionary<int, SO_Conditions>();
        }
    }

    private void Start()
    {
        Init();
    }

    // TODO: think about how to incoporate checking for conditions with adding items.
    // Maybe don't need to if we implicitly decide items can only be added to inventory where
    // slots can hold any items.
    public void AddItem(Item item)
    {
        int emptySlot = inventory.GetFirstEmptySlot();

        if (emptySlot == -1)
        {
            // Inventory Full!
            Debug.Log("Inventory Full! Failed to add item");
            return;
        }

        inventory.AddItem(emptySlot, item);
    }

    public void RemoveItem(Item item)
    {
        int itemIndex = inventoryData.items.LastIndexOf(item.data);

        if (itemIndex == -1)
        {
            // Item not found in inventory!
            Debug.Log("Item not found in inventory! Failed to remove item");
            return;
        }
        inventory.RemoveItem(itemIndex);
    }

    /// <summary>
    /// Switch items in the same inventory system
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    public void SwapItems(int index1, int index2)
    {
        SO_Item item1 = inventory.GetItem(index1);
        SO_Item item2 = inventory.GetItem(index2);

        // Check swap conditions.
        if (!CheckConditionMet(this, index1, item2) || !CheckConditionMet(this, index2, item1))
        {
            // Condition not met!
            Debug.Log("Condition not met! Item cannot be swapped.");
            return;
        }

        inventory.SwapItems(index1, index2);
    }

    /// <summary>
    /// Switch items between two inventory systems
    /// </summary>
    /// <param name="other"></param>
    /// <param name="otherIndex"></param>
    /// <param name="thisIndex"></param>
    public void SwapItemsBetweenSystems(InventorySystem other, int otherIndex, int thisIndex)
    {
        // Get temp of items to swap.
        SO_Item tempThis = inventory.GetItem(thisIndex);
        SO_Item tempOther = other.inventory.GetItem(otherIndex);

        // Check swap conditions.
        if (!CheckConditionMet(this, thisIndex, tempOther) || !CheckConditionMet(other, otherIndex, tempThis))
        {
            // Condition not met!
            Debug.Log("Condition not met! Item cannot be swapped.");
            return;
        }

        // Swap items.
        inventory.RemoveItem(thisIndex);
        inventory.AddItem(thisIndex, tempOther);

        other.inventory.RemoveItem(otherIndex);
        other.inventory.AddItem(otherIndex, tempThis);
    }

    /// <summary>
    /// Attempt to stack items, if not possible, then swap items. (same inventory system)
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    public void AttemptStackThenSwap(int index1, int index2)
    {
        SO_Item item1 = inventory.GetItem(index1);
        SO_Item item2 = inventory.GetItem(index2);

        // Check if items can be stacked.
        if (item1 != null && item2 != null && 
            item1.itemName == item2.itemName && 
            item1.isStackable && item2.isStackable)
        {
            // Stack items.
            inventory.StackItems(index1, index2);
        }
        else
        {
            // Attempt Swap items. (Stacking not possible)
            SwapItems(index1, index2);
        }
    }

    /// <summary>
    /// Attempt to stack items, if not possible, then swap items. (between two inventory systems)
    /// </summary>
    /// <param name="other"></param>
    /// <param name="otherIndex"></param>
    /// <param name="thisIndex"></param>
    public void AttemptStackThenSwapBetweenSystems(InventorySystem other, int otherIndex, int thisIndex)
    {
        SO_Item item1 = inventory.GetItem(thisIndex);
        SO_Item item2 = other.inventory.GetItem(otherIndex);
        // Check if items can be stacked.
        if (item1 != null && item2 != null &&
            item1.itemName == item2.itemName &&
            item1.isStackable && item2.isStackable)
        {
            // Stack items.
            inventory.StackItems(thisIndex, otherIndex);
        }
        else
        {
            // Attempt Swap items. (Stacking not possible)
            SwapItemsBetweenSystems(other, otherIndex, thisIndex);
        }
    }

    /// <summary>
    /// Split item from index1 into index2 in same inventory system.
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    public void Split(int index1, int index2)
    {
        SO_Item item1 = inventory.GetItem(index1);
        SO_Item item2 = inventory.GetItem(index2);

        // Check if item at index2 is null.
        if (item2 != null)
        {
            // Item at index2 is not null!
            Debug.Log("Item at index2 is not null! Failed to split item.");
            return;
        }

        // Check if item is splittable.
        if (!item1.isStackable || item1.quantity <= 1)
        {
            // Item is not splittable!
            Debug.Log("Item is not splittable! Failed to split item.");
            return;
        }

        // Split item.
        SO_Item secondHalf = inventory.SplitIndex(index1);
        inventory.AddItem(index2, secondHalf);
    }


    // HELPER. Checks if an insert condition is met for a slot.
    private bool CheckConditionMet(InventorySystem system, int slotIndex, SO_Item itemToInsert)
    {
        system.GetSlotRules().TryGetValue(slotIndex, out SO_Conditions conditions);
        
        if (conditions == null || itemToInsert == null) {
            // No conditions or no item to insert.
            return true;
        }

        foreach (ConditionType conditionType in conditions.conditionTypes)
        {
            ICondition condition = ConditionTypeTranslator.Instance.Translate(conditionType);
            if (!condition.ConditionMet(itemToInsert))
            {
                // Condition not met!
                Debug.Log("Condition not met!");
                return false;
            }
        }
        return true;
    }

    // Event linker.
    private void Inventory_OnInventoryDataModified()
    {
        OnInventoryDataModified?.Invoke();
    }

    // Init inventory
    private void Init()
    {
        inventory = new Inventory(inventoryData);
        inventory.OnInventoryDataModified += Inventory_OnInventoryDataModified;
    }

    #region Getters and Setters
    public Dictionary<int, SO_Conditions> GetSlotRules()
    {
        return slotRules;
    }

    public SO_Item GetItem(int index)
    {
        return inventory.GetItem(index);
    }

    public SO_Inventory GetInventoryData()
    {
        return inventoryData;
    }

    public void SetInventoryData(SO_Inventory inventoryData)
    {
        this.inventoryData = inventoryData;
        
        // Recreate inventory object with new inventoryData.
        Init();

        OnInventoryDataReset?.Invoke(inventoryData);
    }
    #endregion  
}
