using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

// Modifies state of UI based on inventory data. Also handles user input.
public class InventoryUI : MonoBehaviour
{
    public UnityEvent OnRerender;

    [SerializeField] private SO_Inventory inventoryData;
    [SerializeField] private GameObject inventorySlotParent;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject inventoryTitle;

    private void Awake()
    {
        Assert.IsNotNull(inventoryData, "Need inventory data for UI to reflect the its state.");
    }

    private void Start()
    {
        Init();
    }

    // Init process consists of:
    // 1. Create inventory slots.
    // 2. (Rendering)
    //      - Assign items to slots.
    //      - Triggering onRerender event.
    public void Init()
    {
        // Create inventory slots.
        while (inventorySlotParent.transform.childCount < inventoryData.slots)
        {
            Instantiate(inventorySlotPrefab, inventorySlotParent.transform);
        }

        Rerender();
    }

    // TODO: On inventory data change, rerender UI.
    public void Rerender()
    {
        // Assing items to slots.
        for (int i = 0; i < inventoryData.slots; i++)
        {
            GameObject slot = inventorySlotParent.transform.GetChild(i).gameObject;
            SlotUI slotUI = slot.GetComponent<SlotUI>();
            Assert.IsNotNull(slotUI, "Slot UI component not found on slot game object.");
            slotUI.SetItem(inventoryData.items[i]);
        }

        OnRerender?.Invoke();
    }
}
