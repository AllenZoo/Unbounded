using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Class that replaces singleton logic for holding relevant global data needed during moving items between different inventory systems.
/// </summary>
[CreateAssetMenu(fileName = "new inventory selection context", menuName = "System/Inventory/InventorySelectionContext")]
public class InventorySelectionContext : ScriptableObject
{
    public event Action<InventorySelectionContext> OnSelect;
    public int SelectedSlotIndex { get { return selectedSlotIndex; } set { selectedSlotIndex = value; OnSelect?.Invoke(this); } }
    public InventorySystem SelectedInventorySystem { get { return selectedSlotInventorySystem; } set { selectedSlotInventorySystem = value; OnSelect?.Invoke(this); } }
    public PointerEventData.InputButton InputButton { get { return inputButton; } set { inputButton = value; OnSelect?.Invoke(this); } }

    [SerializeField, ReadOnly]
    private int selectedSlotIndex = -1;

    [SerializeField, ReadOnly]
    private InventorySystem selectedSlotInventorySystem = null;

    /// <summary>
    /// Left click or right click.
    /// </summary>
    [SerializeField, ReadOnly]
    private PointerEventData.InputButton inputButton;


    public void ResetSelection()
    {
        SelectedSlotIndex = -1;
        SelectedInventorySystem = null;

    }

    public void SetContext(int selectedSlotIndex, InventorySystem selectedInventorySystem, PointerEventData.InputButton inputButton)
    {
        this.selectedSlotIndex = selectedSlotIndex;
        this.selectedSlotInventorySystem = selectedInventorySystem;
        this.inputButton = inputButton;
        OnSelect?.Invoke(this);
    }
}
