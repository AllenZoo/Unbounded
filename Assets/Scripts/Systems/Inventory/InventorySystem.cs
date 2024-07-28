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
    public event Action<Inventory> OnInventoryDataReset;

    //[Header("Init through SO.")]
    //[SerializeField] private SO_Inventory initInventory;
    [SerializeField] private Inventory inventory;

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

    private void Awake()
    {
        // Check that inventory is not null
        Assert.IsNotNull(inventory, "Inventory is null.");
        Assert.IsNotNull(inventory.data, "Inventory does not have proper SO_Inventory.");

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
        OnInventoryDataModified?.Invoke();
    }

    public void RemoveItem(Item item)
    {
        int itemIndex = inventory.LastIndexOf(item);

        if (itemIndex == -1)
        {
            // Item not found in inventory!
            Debug.Log("Item not found in inventory! Failed to remove item");
            return;
        }
        inventory.RemoveItem(itemIndex);
        OnInventoryDataModified?.Invoke();
    }

    /// <summary>
    /// Switch items in the same inventory system
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    public void SwapItems(int index1, int index2)
    {
        Item item1 = inventory.GetItem(index1);
        Item item2 = inventory.GetItem(index2);

        // Check swap conditions.
        if (!CheckConditionMet(this, index1, item2) || !CheckConditionMet(this, index2, item1))
        {
            // Condition not met!
            Debug.Log("Condition not met! Item cannot be swapped.");
            return;
        }
        // Conditions met, swap items.
        inventory.SwapItems(index1, index2);
        OnInventoryDataModified?.Invoke();
    }

    /// <summary>
    /// Switch items between two different inventory systems
    /// </summary>
    /// <param name="other"> the initially selected inventory system </param>
    /// <param name="otherIndex"> index of the 'other' inventory system </param>
    /// <param name="thisIndex"></param>
    public void SwapItemsBetweenSystems(InventorySystem other, int otherIndex, int thisIndex)
    {
        // Get temp of items to swap.
        Item tempThis = inventory.GetItem(thisIndex);
        Item tempOther = other.inventory.GetItem(otherIndex);

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
    /// Attempt to stack items, if not possible, then attempt to swap items. (same inventory system).
    /// If indexes to swap are the same, then do nothing.
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    public void AttemptStackThenSwap(int index1, int index2)
    {
        // Check if same index, if so do nothing.
        if (index1 == index2)
        {
            return;
        }
        
        // Check if General Conditions met for stacking/swapping.
        Item item1 = inventory.GetItem(index1);
        Item item2 = inventory.GetItem(index2);
        if (!CheckConditionMet(this, index1, item2) || !CheckConditionMet(this, index2, item1))
        {
            // Condition not met!
            Debug.Log("Condition not met to swap or stack items!");
            return;
        }


        // Attempt to Stack items.
        int stackResult = inventory.StackItems(index1, index2);
        if (stackResult == 1)
        {
            // Stack successful, remove item from other inventory.
            inventory.RemoveItem(index1);
        } else
        {
            // Attempt Swap items. (Stacking not possible)
            SwapItems(index1, index2);
        }
    }

    /// <summary>
    /// Attempt to stack items, if not possible, then swap items. (between two inventory systems)
    /// </summary>
    /// <param name="other"> initially selected inventory system </param>
    /// <param name="otherIndex"> index that belongs to 'other' </param>
    /// <param name="thisIndex"> index item is dropped on. </param>
    public void AttemptStackThenSwapBetweenSystems(InventorySystem other, int otherIndex, int thisIndex)
    {
        
        // Check if General Conditions met for stacking/swapping.
        Item thisItem = inventory.GetItem(thisIndex);
        Item otherItem = other.inventory.GetItem(otherIndex);
        if (!CheckConditionMet(this, thisIndex, otherItem) || !CheckConditionMet(other, otherIndex, thisItem))
        {
            // Condition not met!
            Debug.Log("Condition not met to swap or stack items!");
            return;
        }

        // Attempt to Stack items.
        if (inventory.AddItem(thisIndex, otherItem) == 1)
        {
            // Stack successful, remove item from other inventory.
            other.inventory.RemoveItem(otherIndex);
        }
        else
        {
            // Attempt Swap items. (Stacking not possible)
            SwapItemsBetweenSystems(other, otherIndex, thisIndex);
        }
    }

    /// <summary>
    /// Split item from index1 into index2 in same inventory system.
    /// index2 must contain no item (null), unless it contains the same item 
    /// as index1 and is stackable, which then leads to the behaviour of the moving
    /// split half of the item being stacked onto the item at index2. If these conditions do not match,
    /// nothing happens.
    /// </summary>
    /// <param name="index1"></param>
    /// <param name="index2"></param>
    public void SplitInto(int index1, int index2)
    {
        Item item1 = inventory.GetItem(index1);
        Item item2 = inventory.GetItem(index2);

        // Check if items are swappable
        if (!CheckConditionMet(this, index1, item2) || !CheckConditionMet(this, index2, item1))
        {
            // Condition not met!
            Debug.Log("Condition not met to split items!");
            return;
        }

        // Check if item1 is splittable
        if (item1 == null || item1.data == null || !item1.data.isStackable)
        {
            // item1 is null or not splittable!
            Debug.Log("Item at index1 is null or not splittable! Failed to split item.");
            return;
        }

        // Check if item at index2 is null or atleast stackable and matches item at index 1.
        if (item2 != null && item2.data != null 
            && (!item2.data.isStackable || !item2.data.Equals(item1.data)))
        {
            // item2 is not null and is not stackable with item1!
            Debug.Log("Item at index2 is not null and is not stackable with item1! Failed to split item.");
            return;
        }

        // Attempt to split item.
        Item secondHalf = inventory.SplitIndex(index1);

        if (secondHalf == null)
        {
            // Split failed!
            Debug.Log("Split failed! Failed to split item.");
            return;
        } else
        {
            // Split successful, add second half to index2.
            inventory.AddItem(index2, secondHalf);
        }
    }

    /// <summary>
    /// Same as SplitInto(), but between two inventory systems.
    /// </summary>
    /// <param name="other"> initially selected inventory system </param>
    /// <param name="otherIndex"> index that belongs to 'other'. Also the item index we're splitting on </param>
    /// <param name="thisIndex"> index of item that is dropped on. </param>
    public void SplitIntoBetweenSystems(InventorySystem other, int otherIndex, int thisIndex)
    {
        Item thisItem = inventory.GetItem(thisIndex);
        Item otherItem = other.inventory.GetItem(otherIndex);

        // Check if items are swappable
        if (!CheckConditionMet(this, thisIndex, otherItem) 
            || !CheckConditionMet(other, otherIndex, thisItem))
        {
            // Condition not met!
            Debug.Log("Condition not met to split items!");
            return;
        }

        // Check if item at index2 is null or atleast stackable and matches item at index 1.
        if (otherItem != null && otherItem.data != null && 
            (!otherItem.data.isStackable || otherItem.data.Equals(thisItem.data)))
        {
            // item2 is not null and is not stackable with item1!
            Debug.Log("Item at index2 is not null and is not stackable with item1! Failed to split item.");
            return;
        }

        // Split item. (otherIndex is the index we're splitting on)
        // Attempt to split item.
        Item secondHalf = other.inventory.SplitIndex(otherIndex);
        if (secondHalf == null)
        {
            // Split failed!
            Debug.Log("Split failed! Failed to split item.");
            return;
        }
        else
        {
            // Split successful, add second half to index2.
            inventory.AddItem(thisIndex, secondHalf);
        }
    }

    /// <summary>
    /// HELPER. Checks if an insert condition is met for a slot. 
    /// </summary>
    /// <param name="system">system which slot belongs to</param>
    /// <param name="slotIndex">slot to insert into</param>
    /// <param name="itemToInsert">item to insert into system</param>
    /// <returns>Returns false if item cannot be inserted into slot. Returns true if it can.</returns>
    private bool CheckConditionMet(InventorySystem system, int slotIndex, Item itemToInsert)
    {
        system.GetSlotRules().TryGetValue(slotIndex, out SO_Conditions conditions);
        
        if (conditions == null || itemToInsert == null) {
            // No conditions or no item to insert.
            return true;
        }

        foreach (ConditionType conditionType in conditions.conditionTypes)
        {
            IItemCondition condition = ConditionTypeTranslator.Instance.Translate(conditionType);
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
    private void InvokeInventorySystemOnInventoryModified()
    {
        OnInventoryDataModified?.Invoke();
    }

    // Init
    private void Init()
    {
        inventory.OnInventoryDataModified += InvokeInventorySystemOnInventoryModified;
    }

    #region Getters and Setters
    public Dictionary<int, SO_Conditions> GetSlotRules()
    {
        return slotRules;
    }

    public Item GetItem(int index)
    {
        return inventory.GetItem(index);
    }

    public int GetInventorySize()
    {
        return inventory.GetInventorySize();
    }

    public Inventory GetInventoryData()
    {
        return inventory;
    }

    public void SetInventoryData(Inventory inventory)
    {
        this.inventory = inventory;
        
        // Recreate inventory object with new inventoryData.
        Init();
        OnInventoryDataModified?.Invoke();
        OnInventoryDataReset?.Invoke(inventory);
    }
    #endregion  
}
