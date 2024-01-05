using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Modifies state of UI based on inventory data. Also handles user input.
// Needs inventory system to get data to display.
[RequireComponent(typeof(InventorySystem))]
public class InventoryUI : MonoBehaviour
{
    [SerializeField] public GameObject inventoryMouseFollower;

    // Only invoked in Rerender().
    public UnityEvent OnRerender;
   
    [SerializeField] private GameObject inventorySlotParent;
    [SerializeField] private GameObject inventorySlotPrefab;

    [Tooltip("Should we clear all existing slots and generate using pfb?")]
    [SerializeField] private bool shouldGenerateSlots = true;
    [SerializeField] private GameObject inventoryTitle;

    private List<SlotUI> slots = new List<SlotUI>();
    private InventorySystem inventorySystem;
    private SO_Inventory inventoryData;

    private void Awake()
    {
        if (shouldGenerateSlots)
        {
            Assert.IsNotNull(inventorySlotParent, "Need inventory slot parent to instantiate slots.");
            Assert.IsNotNull(inventorySlotPrefab, "Need inventory slot prefab to instantiate slots.");
        }

        Assert.IsNotNull(inventoryMouseFollower.GetComponent<ItemHoverer>(), "Inventory mouse follower needs ItemHoverer component.");
        Assert.IsNotNull(inventoryMouseFollower.GetComponent<MouseHover>(), "Inventory mouse follower needs MouseHover component.");
        
        inventorySystem = GetComponent<InventorySystem>();
        inventoryData = inventorySystem.GetInventoryData();

        Assert.IsNotNull(inventoryData, "Need inventory data for UI to reflect the its state.");

        InitWhole();
    }

    private void Start()
    {
        // Assign Event Listeners
        foreach (SlotUI slot in slots)
        {
            slot.OnDragItem += OnSlotDrag;
            slot.OnEndDragItem += OnSlotEndDrag;
            slot.OnDropItem += OnSlotDrop;
        }

        inventorySystem.OnInventoryDataModified += Rerender;
        inventorySystem.OnInventoryDataReset += SetInventoryData;
    }

    // For when Inventory UI is closed and reopened.
    private void OnEnable()
    {
        Rerender();
    }


    // Modify selected slot index to slot that is being dragged.
    public void OnSlotDrag(InventorySystem system, SlotUI slot)
    {
        InventorySwapperManager.Instance.selectedSlotIndex  = slot.GetSlotIndex();
        InventorySwapperManager.Instance.selectedSlotInventorySystem = system;
    }

    // Reset selected slot index.
    public void OnSlotEndDrag(InventorySystem system, SlotUI slot)
    {
        InventorySwapperManager.Instance.ResetSelection();
    }

    // Swap items in slot
    public void OnSlotDrop(InventorySystem system, SlotUI slot)
    {
        // Check if selected slot index is valid.
        if (InventorySwapperManager.Instance.selectedSlotIndex == -1)
        {
            return;
        }

        Assert.IsNotNull(InventorySwapperManager.Instance.selectedSlotInventorySystem, "Selected slot inventory system is null.");
        // Check if item dropped on same system or different system.
        if (system != InventorySwapperManager.Instance.selectedSlotInventorySystem)
        {
            // Different system, swap externally. (Could make this an event call, but would require extra checks in Swap function)
            system.SwapItemsBetweenSystems(
                InventorySwapperManager.Instance.selectedSlotInventorySystem,
                InventorySwapperManager.Instance.selectedSlotIndex, 
                slot.GetSlotIndex());
            InventorySwapperManager.Instance.ResetSelection();
        } else
        {
            // Same System, swap internally.
            system.SwapItems(InventorySwapperManager.Instance.selectedSlotIndex, slot.GetSlotIndex());
            InventorySwapperManager.Instance.ResetSelection();
        }
    }


    private void InitWhole()
    {
        // Generates slot gameobjects.
        if (shouldGenerateSlots)
        {
            GenerateSlots();
        }

        // Assign logic to each gameobject slot.
        InitSlots();

        // Assign item to each slot and rerender it.
        Rerender();
    }

    // InitSlots process consists of:
    // 1. Assign index to slots.
    // 2. Assign instance of itemHoverer to each slot.
    // 3. Add slots to list. (init slotUI list)
    private void InitSlots()
    {
        slots.Clear();

        // Assign script logic to each slot.
        for (int i = 0; i < inventorySlotParent.transform.childCount; i++)
        {
            GameObject slot = inventorySlotParent.transform.GetChild(i).gameObject;
            SlotUI slotUI = slot.GetComponent<SlotUI>();
            Assert.IsNotNull(slotUI, "Slot UI component not found on slot game object.");
            slotUI.SetSlotIndex(i);
            slotUI.SetItemHoverer(inventoryMouseFollower.GetComponent<ItemHoverer>());

            slots.Add(slotUI);
        }

    }

    // Clear and generate slots
    private void GenerateSlots()
    {
        // TODO: if doing this, change the way we generate slots
        // by using a for loop on inventoryData.slots

        // Clear pre-existing slots in inventorySlotParent
        //foreach (Transform child in inventorySlotParent.transform)
        //{
        //    Destroy(child.gameObject);
        //    //Debug.Log("Destroying obj!");
        //}

        // Create inventory slots gameobjects.
        while (inventorySlotParent.transform.childCount < inventoryData.slots)
        {
            Instantiate(inventorySlotPrefab, inventorySlotParent.transform);
        }
    }

    // On inventory data change, rerender UI.
    // Sets item in each slot and rerenders it.
    private void Rerender()
    {
        // Check if InventoryUI is active
        if (!gameObject.activeSelf)
        {
            return;
        }

        // Assiging items to slots.
        int index = 0;
        foreach (SlotUI slot in slots)
        {
            slot.SetItem(inventoryData.items[index]);
            index++;
        }

        OnRerender?.Invoke();
    }


    #region Getters and Setters
    public SO_Inventory GetInventoryData()
    {
        return inventoryData;
    }

    public void SetInventoryData(SO_Inventory inventoryData)
    {
        this.inventoryData = inventoryData;
        InitWhole();
    }
    #endregion
}
