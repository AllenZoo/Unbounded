using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private SO_Item itemData;
    [SerializeField] private GameObject itemIconElement;
    private InventoryUI inventoryUI;
    [SerializeField] private int slotIndex;
    private ItemHoverer itemHoverer;

    private void Awake()
    {
        Assert.IsNotNull(itemIconElement, "Need item data sprite gameobject.");
        inventoryUI = GetComponentInParent<InventoryUI>();
    }

    private void Start()
    {
        inventoryUI.OnRerender.AddListener(Rerender);
    }

    // Handles rerendering the item sprite of a slot.
    private void Rerender()
    {
        // Check if slot holds an item.
        if (itemData == null)
        {
            itemIconElement.SetActive(false);
            return;
        }

        itemIconElement.SetActive(true);
        Image image = itemIconElement.GetComponentInParent<Image>();
        Assert.IsNotNull(image, "item icon element needs image component to display item sprite on.");
        image.sprite = itemData.itemSprite;
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    public void SetItemHoverer(ItemHoverer itemHoverer)
    {
        this.itemHoverer = itemHoverer;
    }

    public void SetItem(SO_Item item)
    {
        itemData = item;
        Rerender();
    }


    // On Begin Drag
    // 1. If item is selected in slot, hold it to mouse.
    //    Mouse needs info about:
    //      - the slot index
    //      - the item in slot.
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Got into pointer drag event!");
        if (itemData == null)
        {
            return;
        }

        Assert.IsNotNull(itemHoverer, "Need reference to item hoverer.");
        // Set Data
        itemHoverer.SetData(itemData);
        itemHoverer.SetSlotIndex(slotIndex);
    }

    // On End Drag (check if this calls first, or OnDrop)
    // 1. Disable mouse follower.
    // 2. 
    public void OnEndDrag(PointerEventData eventData)
    {
    }

    // On Drop
    // 1. If item is dropped on slot, swap items.
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Mouse released over slot index: " + slotIndex);
    }
}
