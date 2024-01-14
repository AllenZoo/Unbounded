using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemDescriptor : Singleton<ItemDescriptor>
{
    public SO_Item item;

    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI itemTextName;
    [SerializeField] private TextMeshProUGUI itemTextDesc;
    
    private new void Awake()
    {
        base.Awake();
        Assert.IsNotNull(itemTextName, "Item descriptor needs a reference to a TextMeshProUGUI to display" +
            "item name.");
        Assert.IsNotNull(itemTextDesc, "Item descriptor needs a reference to a TextMeshProUGUI to display" +
            "item description.");
    }

    public void Toggle(bool toggle, SO_Item item)
    {
        Toggle(toggle);
        this.item = item;
        itemTextName.text = item.itemName;
        itemTextDesc.text = item.description;
    }

    public void Toggle(bool toggle)
    {
        gameObject.SetActive(toggle);
    }
}
