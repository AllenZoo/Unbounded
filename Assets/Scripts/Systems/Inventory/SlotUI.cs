using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public event Action<InventorySystem, SlotUI> OnDragItem, OnEndDragItem, OnDropItem;

    private InventoryUI inventoryUI;
    private InventorySystem parentSystem;

    [Header("For debugging, don't set via inspector.")]
    [SerializeField] private int slotIndex;
    [SerializeField] private SO_Item itemData;

    [Header("Set via inspector.")]
    [SerializeField] private GameObject itemIconElement;
    [SerializeField] private GameObject slotItemBackground;

    private void Awake()
    {
        Assert.IsNotNull(itemIconElement, "Need item data sprite gameobject.");
        inventoryUI = GetComponentInParent<InventoryUI>();

        parentSystem = GetComponentInParent<InventorySystem>();
        Assert.IsNotNull(parentSystem, "SlotUI needs reference to parent inventory system.");
    }

    private void Start()
    {
        inventoryUI.OnRerender.AddListener(Rerender);
        Rerender();
    }

    // Handles rerendering the item sprite of a slot.
    private void Rerender()
    {
        // Check if slot holds an item.
        if (itemData == null)
        {
            if (slotItemBackground != null)
            {
                slotItemBackground.SetActive(true);
            }

            itemIconElement.SetActive(false);
            return;
        }

        // Slot holds an item
        if (slotItemBackground != null)
        {
            slotItemBackground.SetActive(false);
        }

        itemIconElement.SetActive(true);
        Image image = itemIconElement.GetComponentInParent<Image>();
        Assert.IsNotNull(image, "item icon element needs image component to display item sprite on.");
        image.sprite = itemData.itemSprite;

        itemIconElement.transform.rotation =  Quaternion.Euler(0f, 0f, itemData.spriteRot);
    }

    // On Begin Drag
    // 1. If item is selected in slot, hold it to mouse.
    //    Mouse needs info about:
    //      - the slot index
    //      - the item in slot.
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Debug.Log("Got into pointer drag event!");
        if (itemData == null)
        {
            return;
        }

        OnDragItem?.Invoke(parentSystem, this);
    }

    // On End Drag (check if this calls first, or OnDrop)
    // 1. Disable mouse follower.
    // 2. 
    public void OnEndDrag(PointerEventData eventData)
    {
        OnEndDragItem?.Invoke(parentSystem, this);
    }

    // On Drop
    // 1. If item is dropped on slot, swap items.
    public void OnDrop(PointerEventData eventData)
    {
        // Debug.Log("Mouse released over slot index: " + slotIndex);
        OnDropItem?.Invoke(parentSystem, this);
    }

    #region Setters and Getters
    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }
    public int GetSlotIndex()
    {
        return slotIndex;
    }
    public void SetItem(SO_Item item)
    {
        itemData = item;
        Rerender();
    }
    public Sprite GetItemSprite()
    {
        return itemData.itemSprite;
    }
    public float GetItemSpriteRot()
    {
        return itemData.spriteRot;
    }
    #endregion
}
