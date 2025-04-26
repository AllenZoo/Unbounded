using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Properties
    public event Action<InventorySystem, SlotUI, PointerEventData.InputButton> OnDragItem;
    public event Action<InventorySystem, SlotUI>  OnEndDragItem, OnDropItem;

    private InventoryUI inventoryUI;
    private InventorySystem parentSystem;

    [Header("For debugging, don't set via inspector.")]
    [SerializeField] private int slotIndex;
    [SerializeField] private Item item;

    [Header("Set via inspector.")]
    [Tooltip("Item icon element that displays item sprite of slot.")]
    [SerializeField] private GameObject itemIconElement;

    [Tooltip("Optional, if you want to display a background for an empty item slot. Eg. dagger background" +
        "for equipement weapon slot.")]
    [SerializeField] private GameObject slotItemBackground;

    [SerializeField] private TextMeshProUGUI quantityText;

    [Header("Slot params")]
    [Tooltip("Is this slot draggable?")]
    [SerializeField] private bool isDraggable = true;

    [Tooltip("Can we drop items onto this slot?")]
    [SerializeField] private bool isDroppable = true;

    [Header("Hover params")]
    [Tooltip("Delay before item description is shown.")]
    [SerializeField] private float hoverDelay = 0.5f; // Adjust the delay time as needed
    private bool isMouseOver = false;
    private Coroutine hoverCoroutine;
    #endregion

    private void Awake()
    {
        Assert.IsNotNull(itemIconElement, "Need item data sprite gameobject.");
        inventoryUI = GetComponentInParent<InventoryUI>();
        Assert.IsNotNull(inventoryUI, "ERROR: Slot UI existing without parent Inventory UI reference!");

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
        if (item == null || item.data == null)
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
        Image image = itemIconElement.GetComponent<Image>();
        Assert.IsNotNull(image, "item icon element needs image component to display item sprite on.");
        image.sprite = item.data.itemSprite;
        itemIconElement.transform.rotation =  Quaternion.Euler(0f, 0f, item.data.spriteRot);

        // Check if item is stackable and if quantity is greater than 1.
        if (item.data.isStackable && item.quantity > 1)
        {
            quantityText.text = "x" + item.quantity.ToString();
            quantityText.gameObject.SetActive(true);
        }
        else
        {
            quantityText.gameObject.SetActive(false);
        }

        // Add gray tint to slot if slot is uninteractive
        if (!isDraggable || !isDroppable)
        {
            image.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        }
        else
        {
            image.color = Color.white;
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
        if (item == null || item.data == null)
        {
            return;
        }

        if (!isDraggable)
        {
            return;
        }

        OnDragItem?.Invoke(parentSystem, this, eventData.button);
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
        if (!isDroppable)
        {
            return;
        }
        OnDropItem?.Invoke(parentSystem, this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isMouseOver && item != null && item.data != null)
        {
            isMouseOver = true;
            hoverCoroutine = StartCoroutine(DelayedItemDescriptor());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isMouseOver)
        {
            isMouseOver = false;
            if (hoverCoroutine != null)
            {
                StopCoroutine(hoverCoroutine);
            }
            // Hide the item descriptor here (e.g., set it inactive)
            inventoryUI.SetItemDescriptor(null, false);
        }
    }

    public void ToggleSlotInteractivity(bool isInteractive)
    {
        isDraggable = isInteractive;
        isDroppable = isInteractive;
        Rerender();
    }

    private IEnumerator DelayedItemDescriptor()
    {
        yield return new WaitForSeconds(hoverDelay);
        // Show the item descriptor here (e.g., set it active)
        inventoryUI.SetItemDescriptor(item, true);
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
    public void SetItem(Item item)
    {
        this.item = item;
        Rerender();
    }
    public Sprite GetItemSprite()
    {
        return item.data.itemSprite;
    }
    public float GetItemSpriteRot()
    {
        return item.data.spriteRot;
    }
    #endregion
}
