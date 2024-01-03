using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Modifies state of UI based on inventory data. Also handles user input.
public class InventoryUI : MonoBehaviour
{
    [SerializeField] public GameObject inventoryMouseFollower;

    // Only invoked in Rerender().
    public UnityEvent OnRerender;
    // Within the same inventory.
    public event Action<int, int> OnSwapItems;
   

    [SerializeField] private SO_Inventory inventoryData;
    [SerializeField] private GameObject inventorySlotParent;
    [SerializeField] private GameObject inventorySlotPrefab;

    [Tooltip("When slots on ui # is less than slots in inventory, should generate slots? Generally" +
        "true unless we want to manually make the slots.")]
    [SerializeField] private bool shouldGenerateSlots = true;
    [SerializeField] private GameObject inventoryTitle;

    private List<SlotUI> slots = new List<SlotUI>();

    //private int selectedSlotIndex = -1;
    //private InventorySystem selectedSlotInventorySystem = null;


    private void Awake()
    {
        Assert.IsNotNull(inventoryData, "Need inventory data for UI to reflect the its state.");

        if (shouldGenerateSlots)
        {
            Assert.IsNotNull(inventorySlotParent, "Need inventory slot parent to instantiate slots.");
            Assert.IsNotNull(inventorySlotPrefab, "Need inventory slot prefab to instantiate slots.");
        }

        Assert.IsNotNull(inventoryMouseFollower.GetComponent<ItemHoverer>(), "Inventory mouse follower needs ItemHoverer component.");
        Assert.IsNotNull(inventoryMouseFollower.GetComponent<MouseHover>(), "Inventory mouse follower needs MouseHover component.");
        Init();
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
    }

    // For when Inventory UI is closed and reopened.
    private void OnEnable()
    {
        Rerender();
    }

    // Init process consists of:
    // 1. Create inventory slots.
    // 2. Assign index to slots.
    // 3. Assign instance of itemHoverer to each slot.
    // 4. Add slots to list. (init slotUI list)
    // 5. (Rendering)
    //      - Assign items to slots.
    //      - Triggering onRerender event.
    public void Init()
    {
        // Create inventory slots.
        while (inventorySlotParent.transform.childCount < inventoryData.slots && shouldGenerateSlots)
        {
            Instantiate(inventorySlotPrefab, inventorySlotParent.transform);
        }

        for (int i = 0; i < inventorySlotParent.transform.childCount; i++)
        {
            GameObject slot = inventorySlotParent.transform.GetChild(i).gameObject;
            SlotUI slotUI = slot.GetComponent<SlotUI>();
            Assert.IsNotNull(slotUI, "Slot UI component not found on slot game object.");
            slotUI.SetSlotIndex(i);
            slotUI.SetItemHoverer(inventoryMouseFollower.GetComponent<ItemHoverer>());

            slots.Add(slotUI);
        }

        Rerender();
    }

    // TODO: On inventory data change, rerender UI. Currently done manually in InventorySystem.
    public void Rerender()
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
            OnSwapItems?.Invoke(InventorySwapperManager.Instance.selectedSlotIndex, slot.GetSlotIndex());
            InventorySwapperManager.Instance.ResetSelection();
        }
    }


    public SO_Inventory GetInventoryData()
    {
        return inventoryData;
    }
}
