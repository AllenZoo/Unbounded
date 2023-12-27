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
    }

    private void Start()
    {
        inventory = new Inventory(inventoryData);
        // onInventoryDataChange += inventoryUI.Rerender;
    }

    public void AddItem(Item item)
    {
        int emptySlot = inventory.GetFirstEmptySlot();
        inventory.AddItem(emptySlot, item);
        // onInventoryDataChange();
    }

    public void RemoveItem(Item item)
    {
        inventory.RemoveItem(inventoryData.items.IndexOf(item.data));
    }
}
