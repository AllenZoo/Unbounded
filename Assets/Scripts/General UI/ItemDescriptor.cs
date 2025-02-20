using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        HandleItemDisplay(item);
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

    private String StringifyStatModifierList(List<StatModifierEquipment> list)
    {
        HashSet<Stat> visitedStats = new HashSet<Stat>();
        List<StatModifier> converted = list.Select(mod => mod.GetModifier()).ToList();
        IStatModifierApplicationOrder order = new NormalStatModifierOrder();

        String statModifierString = "";

        while (converted.Count > 0)
        {
            Stat statToQuery = converted[0].Stat;
            StatQuery statQuery = new StatQuery(statToQuery, 0);
            StatMediator.CalculateFinalStat(converted, statQuery, order);

            visitedStats.Add(statToQuery);
            converted.RemoveAll(mod => (visitedStats.Contains(mod.Stat)));

            if (statQuery.Value <= 0)
            {
                continue;
            }

            statModifierString += statQuery.Stat.ToString() + ": +" + statQuery.Value.ToString() + "\n";
        }

        return statModifierString;
    }

    private void HandleItemDisplay(Item item)
    {

        if (item.HasComponent<ItemBaseStatComponent>())
        {
            itemTextStats.text += "Base Stats:\n";
            ItemBaseStatComponent itemBaseStatComponent = item.GetComponent<ItemBaseStatComponent>();
            itemTextStats.text += StringifyStatModifierList(itemBaseStatComponent.statModifiers);
        }

        if (item.HasComponent<ItemUpgradeComponent>())
        {
            if (item.GetComponent<ItemUpgradeComponent>().upgradeStatModifiers.Count > 0)
            {
                // Hide this text if no upgrades are present.
                itemTextStats.text += "Upgrades:\n";
            }
            ItemUpgradeComponent itemUpgradeComponent = item.GetComponent<ItemUpgradeComponent>();
            itemTextStats.text += StringifyStatModifierList(itemUpgradeComponent.upgradeStatModifiers);
        }

        if (item.HasComponent<ItemUpgraderComponent>())
        {
            itemTextStats.text += "Upgrader:\n";
            ItemUpgraderComponent itemUpgraderComponent = item.GetComponent<ItemUpgraderComponent>();
            itemTextStats.text += StringifyStatModifierList(itemUpgraderComponent.modifiers);
        }
    }
}
