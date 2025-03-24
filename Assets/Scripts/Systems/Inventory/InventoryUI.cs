using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Modifies state of UI based on inventory data. Also handles user input.
// Needs inventory system to get data to display.
// TODO-OPT(major change): decouple inventorySystem from ui and just refer to each other through the Inventory Data Scriptable Object.
//                         this however would also require sharing a LocalEventHandler between the two scripts for event handling. 
//                         This could potentially be handled by storing that reference in the SO_InventoryData by making LocalEventHandler
//                         have a non-monobehaviour version.
//                         
//                         Another option to approach above is to have UI subscribe to some OnInventoryModified in Inventory_SO and have the system 
//                        call that event when it modifies the data. This would require the system to have a reference to the Inventory_SO.
[RequireComponent(typeof(InventorySystem))]
public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// The context shared by Inventory UI systems that exist in the same system. Useful for keeping track of selected objects, and inventories in the middle of
    /// the dragging phase, to allow for swapping items between different Inventory UI.
    /// 
    /// A global variable encapsulated by SO. UI's that share the same system, will share the same scriptable object.
    /// </summary>
    [Required, SerializeField] 
    private InventorySelectionContext InventorySelectionContext;

    /// <summary>
    /// Shared context for displaying the proper item sprite onto relevant UI. eg. Mouse follower UI.
    /// </summary>
    [Required, SerializeField]
    private ItemSelectionContext ItemSelectionContext;

    /// <summary>
    /// Shared context for displaying item descriptor UI.
    /// </summary>
    [Required, SerializeField]
    private ItemDescriptorContext ItemDescriptorContext;

    // Only invoked in Rerender().
    public UnityEvent OnRerender;
   
    [SerializeField] private GameObject inventorySlotParent;
    [SerializeField] private GameObject inventorySlotPrefab;

    [Tooltip("Should we clear all existing slots and generate using pfb?")]
    [SerializeField] private bool shouldGenerateSlots = true;

    private List<SlotUI> slots = new List<SlotUI>();
    private InventorySystem inventorySystem;
    //private Inventory inventory;

    private void Awake()
    {
        if (shouldGenerateSlots)
        {
            Assert.IsNotNull(inventorySlotParent, "Need inventory slot parent to instantiate slots.");
            Assert.IsNotNull(inventorySlotPrefab, "Need inventory slot prefab to instantiate slots.");
        }

        Assert.IsNotNull(InventorySelectionContext, "Warning: Inventory Selection Context is null!");
        Assert.IsNotNull(ItemSelectionContext, "Warning: Item Selection Context is null!");
        Assert.IsNotNull(ItemDescriptorContext, "Warning: Item Descriptor Context is null!");

        inventorySystem = GetComponent<InventorySystem>();

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

        EventBinding<OnInventoryModifiedEvent> inventoryChangeBinding = new EventBinding<OnInventoryModifiedEvent>(Rerender);
        EventBus<OnInventoryModifiedEvent>.Register(inventoryChangeBinding);
        //inventorySystem.OnInventoryDataReset += SetInventoryData;
    }

    // For when Inventory UI is closed and reopened.
    private void OnEnable()
    {
        Rerender();
    }


    // Modify selected slot index to slot that is being dragged.
    public void OnSlotDrag(InventorySystem system, SlotUI slot, PointerEventData.InputButton input)
    {
        InventorySelectionContext?.SetContext(slot.GetSlotIndex(), system, input);

        // Set mouse follower active. Assign sprite to mouse follower.
        ItemSelectionContext?.SetItemSelectionContext(slot.GetItemSprite(), slot.GetItemSpriteRot(), true);
    }

    // Reset selected slot index.
    public void OnSlotEndDrag(InventorySystem system, SlotUI slot)
    {
        InventorySelectionContext?.ResetSelection();

        // Deactivate mouse follower.
        ItemSelectionContext.ShouldDisplay = false;
    }

    // Swap items in slot
    public void OnSlotDrop(InventorySystem system, SlotUI slot)
    {
        // Check if selected slot index is valid.
        if (InventorySelectionContext?.SelectedSlotIndex == -1)
        {
            return;
        }

        // Check if left click/right click initiated the drag.
        // left click = swap
        // right click = split

        bool shouldSwap = InventorySelectionContext?.InputButton == PointerEventData.InputButton.Left;


        Assert.IsNotNull(InventorySelectionContext?.SelectedInventorySystem, "Selected slot inventory system is null.");
        // Check if item dropped on same system or different system.
        if (system != InventorySelectionContext?.SelectedInventorySystem)
        {
            // Different system
            if (shouldSwap)
            {
                // Swap externally.
                system.AttemptStackThenSwapBetweenSystems(
                    InventorySelectionContext?.SelectedInventorySystem,
                    InventorySelectionContext != null ? InventorySelectionContext.SelectedSlotIndex : -1,
                    slot.GetSlotIndex());
            } else
            {
                // Split externally.
                system.SplitIntoBetweenSystems(
                    InventorySelectionContext?.SelectedInventorySystem,
                    InventorySelectionContext != null ? InventorySelectionContext.SelectedSlotIndex : -1,
                    slot.GetSlotIndex());
            }
            InventorySelectionContext?.ResetSelection();
        } 
        else
        {
            // Same system
            if (shouldSwap)
            {
                // Swap internally.
                system.AttemptStackThenSwap(InventorySelectionContext != null ? InventorySelectionContext.SelectedSlotIndex : -1, slot.GetSlotIndex());
                InventorySelectionContext?.ResetSelection();
            } else
            {
                // Split internally.
                system.SplitInto(InventorySelectionContext != null ? InventorySelectionContext.SelectedSlotIndex : -1, slot.GetSlotIndex());
            }
        }
    }

    public void DisableSlot(int index)
    {
        slots[index].ToggleSlotInteractivity(false);
    }

    public void EnableSlot(int index)
    {
         slots[index].ToggleSlotInteractivity(true);
    }

    /// <summary>
    /// Helper to set the item descriptor. The most ideal design pattern however would be to 
    /// make InventoryUI handle the base call of setting the item descriptor and have SlotUI
    /// simply invoke events like OnPointerHover, etc. But this is a simple fix and since 
    /// the two classes are coupled due to other code, no point in making it less coupled for now.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="shouldDisplay"></param>
    public void SetItemDescriptor(Item item, bool shouldDisplay)
    {
        ItemDescriptorContext.SetItemDescriptorContext(item, shouldDisplay);
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

            // Ignore slots without UI
            if (slotUI == null) continue;
            // Assert.IsNotNull(slotUI, "Slot UI component not found on slot game object.");

            slotUI.SetSlotIndex(i);
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
        while (inventorySlotParent.transform.childCount < inventorySystem.GetInventorySize())
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
            slot.SetItem(inventorySystem.GetItem(index));
            index++;
        }

        OnRerender?.Invoke();
    }

    private void Update()
    {
        // For debugging.
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Rerender
            Rerender();
        }
    }
}
