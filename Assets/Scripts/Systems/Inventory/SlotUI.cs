using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [Tooltip("Item icon element that displays item sprite of slot.")]
    [SerializeField] private GameObject itemIconElement;

    [Tooltip("Optional, if you want to display a background for an empty item slot. Eg. dagger background" +
        "for equipement weapon slot.")]
    [SerializeField] private GameObject slotItemBackground;

    [SerializeField] private TextMeshProUGUI quantityText;

    private void Awake()
    {
        Assert.IsNotNull(itemIconElement, "Need item data sprite gameobject.");
        inventoryUI = GetComponentInParent<InventoryUI>();

        parentSystem = GetComponentInParent<InventorySystem>();
        Assert.IsNotNull(parentSystem, "SlotUI needs reference to parent inventory system.");

        Assert.IsNotNull(quantityText, "Need quantity text to display quantity of item.");
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
            // Slot is empty
            if (slotItemBackground != null)
            {
                slotItemBackground.SetActive(true);
            }

            itemIconElement.SetActive(false);
            quantityText.gameObject.SetActive(false);
            return;
        }

        // Slot holds an item

        // Hide slot background if it exists.
        if (slotItemBackground != null)
        {
            slotItemBackground.SetActive(false);
        }

        // Display item sprite.
        itemIconElement.SetActive(true);
        Image image = itemIconElement.GetComponentInParent<Image>();
        Assert.IsNotNull(image, "item icon element needs image component to display item sprite on.");
        image.sprite = itemData.itemSprite;
        itemIconElement.transform.rotation =  Quaternion.Euler(0f, 0f, itemData.spriteRot);

        // Check if item is stackable and if quantity is greater than 1.
        Debug.Log("Item quantity: " + itemData.quantity);
        if (itemData.isStackable && itemData.quantity > 1)
        {
            Debug.Log("Got in here!");
            quantityText.text = "x" + itemData.quantity.ToString();
            quantityText.gameObject.SetActive(true);
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }
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
