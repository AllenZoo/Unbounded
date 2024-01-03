using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Manages the inventory data. Does not handle UI, but processes requests to add/remove/swap items.
[RequireComponent(typeof(InventoryUI))]
public class InventorySystem : MonoBehaviour
{
    // Events
    // Callbacks for when inventory data changes.
    public delegate void OnInventoryDataChange();
    public event OnInventoryDataChange onInventoryDataChange;

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
    private SO_Inventory inventoryData;
    private Inventory inventory;
    private InventoryUI inventoryUI;

    private void Awake()
    {
        inventoryUI = GetComponent<InventoryUI>();
        inventoryData = inventoryUI.GetInventoryData();

        if (slotRules == null)
        {
            slotRules = new SerializedDictionary<int, SO_Conditions>();
        }
    }

    private void Start()
    {
        inventory = new Inventory(inventoryData);
        onInventoryDataChange += InventorySystem_onInventoryDataChange;

        // Assign Event Listeners
        inventoryUI.OnSwapItems += SwapItems;
    }

    private void InventorySystem_onInventoryDataChange()
    {
        // Debug.Log("On inventory data change event triggered.");
        inventoryUI.Rerender();
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
        onInventoryDataChange?.Invoke();
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
        onInventoryDataChange?.Invoke();
    }

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
        onInventoryDataChange?.Invoke();
    }

    // Switch items between two inventory systems
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

        // Invoke events.
        onInventoryDataChange?.Invoke();
        other.onInventoryDataChange?.Invoke();
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

    #region Getters and Setters
    public Dictionary<int, SO_Conditions> GetSlotRules()
    {
        return slotRules;
    }
    #endregion  
}
