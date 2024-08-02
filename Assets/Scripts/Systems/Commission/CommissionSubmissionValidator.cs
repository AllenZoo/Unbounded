using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Handles validating a commission submission.
/// Checks if the Item to submit is the correct item for the commission.
/// </summary>
public class CommissionSubmissionValidator
{

    public CommissionSubmissionValidator() { 
    }

    /// <summary>
    /// Validates the submission of an item for a commission.
    /// </summary>
    /// <param name="commission"></param>
    /// <param name="item"></param>
    /// <returns>true if valid, and false otherwise.</returns>
    public bool ValidateSubmission(Commission commission, Item item)
    {
        if (item == null || commission == null) return false;

        // Check if the item is the correct equipment type for the commission.
        // Also checks if the item contains a BaseStat and Upgrade component.
        if (!item.HasComponent<ItemEquipmentComponent>() ||
            item.GetComponent<ItemEquipmentComponent>().equipmentType != commission.equipmentType ||
            !item.HasComponent<ItemBaseStatComponent>() ||
            !item.HasComponent<ItemUpgradeComponent>())
        {
            Debug.Log("Item is not the correct type for this commission.");
            return false;
        }

        // Check if the item has enough required stats for the commission.
        foreach(KeyValuePair<Stat, int> requirement in commission.statRequirements)
        {
            // Convert keyvaluepair to tuple
            Tuple<Stat, int> requirementTuple = new Tuple<Stat, int>(requirement.Key, requirement.Value);

            if (!CheckStatRequirements(requirementTuple, item))
            {
                Debug.Log("Item does not meet the required stats for this commission.");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if the item has the required stats given the required stat requirements.
    /// </summary>
    /// <param name="requirement"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private bool CheckStatRequirements(Tuple<Stat, int> requirement, Item item)
    {
        // Stub
        List<StatModifier> itemModifiers = new List<StatModifier>();

        // Get all the stat modifiers from the item.
        itemModifiers.AddRange(item.GetComponent<ItemBaseStatComponent>().statModifiers.Select(component => component.GetModifier()));
        itemModifiers.AddRange(item.GetComponent<ItemUpgradeComponent>().upgradeStatModifiers.Select(component => component.GetModifier()));

        StatQuery query = new StatQuery(requirement.Item1, 0);
        StatMediator.CalculateFinalStat(itemModifiers, query, new NormalStatModifierOrder());
        if (query.Value >= requirement.Item2)
        {
            return true;
        }

        return false;
    }


}
