using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Util class that extracts info from Item object and creates an ItemDescModel object.
/// </summary>
public class ItemDataConverter
{
    public static ItemDescModel ConvertFromItem(Item item)
    {
        ItemDescModel model = new ItemDescModel();

        item.Init();
        var mediator = item.ItemModifierMediator;

        StatContainer baseStatContainer = mediator.GetStatsBeforeModification().Value;
        StatContainer finalStatContainer = mediator.GetStatsAfterModification().Value;
        Attacker finalAttacker = mediator.GetAttackerAfterModification();


        model.Name = item.Data.itemName;
        model.Description = item.Data.description;
        model.BaseAtk = baseStatContainer.Attack;
        model.FinalAtk = finalStatContainer.Attack;
        model.PercentageDamageIncrease = mediator.GetPercentageDamageIncreaseTotal();
        model.ProjectileSpeed = finalAttacker.AttackData.initialSpeed;
        model.ProjectileRange = finalAttacker.AttackData.distance;
        model.FireRate = finalStatContainer.Dexterity; // TODO: calculate this via the DEX stat. Also check if this is accurate (like will it include the player's Dexterity Stat too.)
        model.NumProjectilesPerAttack = finalAttacker.AttackerData.numAttacks;
        model.CanViewUpgrades = item.HasComponent<ItemAttackContainerComponent>();

        model.BonusStats = new List<BonusStatEntry>();
        StatContainer diff = finalStatContainer.Diff(baseStatContainer);
        Dictionary<Stat, float> nonZeroStats = diff.GetNonZeroStats();
        foreach (var entry in nonZeroStats)
        {
            BonusStatEntry bonusStatEntry = new BonusStatEntry();
            bonusStatEntry.stat = entry.Key;
            bonusStatEntry.value = entry.Value;
            model.BonusStats.Add(bonusStatEntry);
        }

        model.Traits = new List<string>(); // TODO: currently only Piercing..
        if (finalAttacker.AttackData.isPiercing)
        {
            model.Traits.Add("Piercing");
        }

        return model;
    }
}
