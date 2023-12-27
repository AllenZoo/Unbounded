using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Manages the inventory data. Does not handle UI, but processes requests to add/remove/swap items.
[RequireComponent(typeof(InventoryUI))]
public class InventorySystem : MonoBehaviour
{
    // Ref to inventory data.
    [SerializeField] private SO_Inventory inventoryData;
    private Inventory inventory;
    private InventoryUI inventoryUI;

    // Events
    // Callbacks for when inventory data changes.
    public delegate void OnInventoryDataChange();
    public event OnInventoryDataChange onInventoryDataChange;


    private void Awake()
    {
        Assert.IsNotNull(inventoryData);
        inventoryUI = GetComponent<InventoryUI>();
    }

    private void Start()
    {
        inventory = new Inventory(inventoryData);
        onInventoryDataChange += InventorySystem_onInventoryDataChange;
    }

    private void InventorySystem_onInventoryDataChange()
    {
        // Debug.Log("On inventory data change event triggered.");
        inventoryUI.Rerender();
    }

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
}
