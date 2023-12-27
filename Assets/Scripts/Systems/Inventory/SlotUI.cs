using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    [SerializeField] private SO_Item itemData;
    [SerializeField] private GameObject itemIconElement;
    private InventoryUI inventoryUI;

    private void Awake()
    {
        inventoryUI = GetComponentInParent<InventoryUI>();
    }

    private void Start()
    {
        inventoryUI.OnRerender.AddListener(Rerender);
    }

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

    public void SetItem(SO_Item item)
    {
        itemData = item;
        Rerender();
    }

}
