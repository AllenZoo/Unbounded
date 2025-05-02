using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Attach to UI that should display relevant info of ItemDescriptorContext.
/// </summary>
public class ItemDescriptor : MonoBehaviour
{
    // TODO: Fix the slight jerking motion when ItemDescriptor is toggled on. 
    //      The issue may be caused by only disabling the display and then re-enabling it, which could affect formatting. 
    //      Previously, this wasn't a problem because the entire ItemDescriptor object was disabled, ensuring the display was always enabled when ItemDescriptor was active.


    [Header("Context")]
    [SerializeField, Required] private ItemDescriptorContext context;

    [Tooltip("The UI object we toggle on and off depending on context visibility.")]
    [Required, SerializeField] private GameObject displayUI; // The actual object we toggle on and off, depending on the selection context.

    [Header("UI elements")]
    [Required, SerializeField, ValidateInput(nameof(ValidateDisplayText), "itemTextName must be a child of displayUI.")] 
    private TextMeshProUGUI itemTextName;

    [Required, SerializeField, ValidateInput(nameof(ValidateDisplayText), "itemTextDesc must be a child of displayUI.")]
    private TextMeshProUGUI itemTextDesc;

    [Required, SerializeField, ValidateInput(nameof(ValidateDisplayText), "itemTextStats must be a child of displayUI.")]
    private TextMeshProUGUI itemTextStats;
    
    private void Awake()
    {
        Assert.IsNotNull(context, "Item Descriptor context is null! This will make it unable to display any relevant info needed.");
        Assert.IsNotNull(itemTextName, "Item descriptor needs a reference to a TextMeshProUGUI to display" +
            "item name.");
        Assert.IsNotNull(itemTextDesc, "Item descriptor needs a reference to a TextMeshProUGUI to display" +
            "item description.");
        Assert.IsNotNull(itemTextStats, "Item descriptor needs a reference to a TextMeshProUGUI to display" +
            " stat modifiers of item.");
    }

    private void Start()
    {
        context.OnItemDescriptorChange += OnItemDescriptorChangeEvent;
        Rerender();
    }

    private void OnItemDescriptorChangeEvent(ItemDescriptorContext context)
    {
        Rerender();
    }

    private void Rerender()
    {
        displayUI.SetActive(context.ShouldDisplay);

        if (context.Item == null || context.Item.IsEmpty()) return;

        itemTextName.text = context.Item.data.itemName;
        itemTextDesc.text = context.Item.data.description;
        itemTextStats.text = "";
        HandleItemDisplay(context.Item);
    }

    #region Helpers

    /// <summary>
    /// Converts a list of StatModifierEquipment into a more readable string format.
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Handles the logic for generating what text to put on descriptor for the STAT text.
    /// </summary>
    /// <param name="item"></param>
    private void HandleItemDisplay(Item item)
    {

        if (item.HasComponent<ItemBaseStatComponent>())
        {
            throw new NotImplementedException();
            itemTextStats.text += "Base Stats:\n";
            ItemBaseStatComponent itemBaseStatComponent = item.GetComponent<ItemBaseStatComponent>();
            // TODO: stringify base stats somehow else.
            //itemTextStats.text += StringifyStatModifierList(itemBaseStatComponent.statModifiers);
        }

        if (item.HasComponent<ItemUpgradeComponent>())
        {
            throw new NotImplementedException();
            //if (item.GetComponent<ItemUpgradeComponent>().upgradeStatModifiers.Count > 0)
            //{
            //    // Hide this text if no upgrades are present.
            //    itemTextStats.text += "Upgrades:\n";
            //}
            //ItemUpgradeComponent itemUpgradeComponent = item.GetComponent<ItemUpgradeComponent>();
            //itemTextStats.text += StringifyStatModifierList(itemUpgradeComponent.upgradeStatModifiers);
        }

        if (item.HasComponent<ItemUpgraderComponent>())
        {
            itemTextStats.text += "Upgrader:\n";
            ItemUpgraderComponent itemUpgraderComponent = item.GetComponent<ItemUpgraderComponent>();
            itemTextStats.text += StringifyStatModifierList(itemUpgraderComponent.modifiers);
        }
    }
    #endregion

    #region Validators
    private bool ValidateDisplayText(GameObject obj)
    {
        if (obj == null || displayUI == null) return false; // Ensures both fields are assigned
        return obj.transform.IsChildOf(displayUI.transform); // Checks if displayImage is a child of displayUI
    }
    #endregion
}
