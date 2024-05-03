using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemDescriptor : Singleton<ItemDescriptor>
{
    public Item item;

    [Header("UI elements")]
    [SerializeField] private TextMeshProUGUI itemTextName;
    [SerializeField] private TextMeshProUGUI itemTextDesc;
    [SerializeField] private TextMeshProUGUI itemTextStats;
    
    private new void Awake()
    {
        Debug.Log("Run (ItemDescriptor)");
        base.Awake();
        Assert.IsNotNull(itemTextName, "Item descriptor needs a reference to a TextMeshProUGUI to display" +
            "item name.");
        Assert.IsNotNull(itemTextDesc, "Item descriptor needs a reference to a TextMeshProUGUI to display" +
            "item description.");
        Assert.IsNotNull(itemTextStats, "Item descriptor needs a reference to a TextMeshProUGUI to display" +
            " stat modifiers of item.");
    }

    public void Toggle(bool toggle, Item item)
    {
        Toggle(toggle);
        this.item = item;
        itemTextName.text = item.data.itemName;
        itemTextDesc.text = item.data.description;

        itemTextStats.text = "";
        foreach (IStatModifier statModifier in item.data.statModifiers)
        {
            itemTextStats.text += StringifyStatModifier(statModifier) + "\n";
        }
    }

    public void Toggle(bool toggle)
    {
        gameObject.SetActive(toggle);

        // Toggle children objects
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(toggle);
        }
    }

    private String StringifyStatModifier(IStatModifier statModifier)
    {
        String statModifierString = "";
        statModifierString += statModifier.Stat.ToString() + ": +" + statModifier.Value.ToString();
        return statModifierString;
    }
}
