using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Util class that extracts info from Item object and creates an ItemDescModel object.
/// </summary>
public class ItemDataConverter
{
    public static ItemDescViewConfig ConvertFromItem(Item item)
    {
        if (item == null || item.IsEmpty())
        {
            return null;
        }

        ItemDescViewConfig model = new ItemDescViewConfig();
        
        var mediator = item.ItemModifierMediator;

        if (mediator == null)
        {
            Debug.LogError("ItemModifierMediator is null for item: " + item.Data.itemName);
            return model;
        }

        StatContainer baseStatContainer = mediator.QueryStatsBeforeModification().Value;
        StatContainer finalStatContainer = mediator.QueryStatsAfterModification().Value;
        IAttacker finalAttacker = mediator.QueryAttackerAfterModification();

        // Get Player Stats
        StatComponent playerStats = null;
        try { playerStats = PlayerSingleton.Instance?.GetPlayerStatComponent(); } catch { }
        
        // Use player's base stats (excluding weapon modifiers) for stable display
        float playerBaseAtk = playerStats != null ? playerStats.StatContainer.GetBaseStatValue(Stat.ATK) : 0f;
        float playerBaseDex = playerStats != null ? playerStats.StatContainer.GetBaseStatValue(Stat.DEX) : 0f;

        model.Name = item.Data.itemName;
        model.Description = item.Data.description;

        model.BaseAtk = baseStatContainer.Attack;
        // Include projectile damage as part of base attack for item.
        float attackerBaseDamage = (finalAttacker != null) ? finalAttacker.BaseDamage : 0f;
        
        // Combine weapon and player base attack. 
        model.BaseAtk = baseStatContainer.Attack + attackerBaseDamage + playerBaseAtk;

        model.BonusAtk = finalStatContainer.Attack - baseStatContainer.Attack;
        model.PlayerAtk = playerBaseAtk;
        model.PlayerDex = playerBaseDex;
        model.PercentageDamageIncrease = (float)mediator.QueryPercentageDamageIncreaseTotal();

        // Calculate the absolute contribution from the percentage bonus for the UI breakdown
        float basePlusBonus = model.BaseAtk + model.BonusAtk;
        model.DamageIncreaseFromPercent = (float)(basePlusBonus * (model.PercentageDamageIncrease / 100f));
        
        // Sum and floor to match Damageable logic
        model.Damage = Mathf.Floor(basePlusBonus + model.DamageIncreaseFromPercent);

        if (finalAttacker != null && finalAttacker.AttackData != null) model.ProjectileSpeed = finalAttacker.AttackData.InitialSpeed;
        if (finalAttacker != null && finalAttacker.AttackData != null) model.ProjectileRange = finalAttacker.AttackData.Distance;
        
        // FireRate calculation using dexterity
        model.FireRate = (finalAttacker != null) ? finalAttacker.GetCooldown(playerBaseDex) : 0f;

        if (finalAttacker != null && finalAttacker.AttackerData != null) model.NumProjectilesPerAttack = finalAttacker.AttackerData.numAttacks;
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


        // TRAITS (add more here when required)
        model.Traits = new List<string>();
        if (finalAttacker != null && finalAttacker.AttackData != null && finalAttacker.AttackData.IsPiercing)
        {
            model.Traits.Add("+ Piercing");
        }
        if (model.PercentageDamageIncrease > 0)
        {
            model.Traits.Add($"% Damage Increase: +{model.PercentageDamageIncrease}%");
        }


        model.weaponImage = item.Data.itemSprite;
        model.weaponImageRot = item.Data.spriteRot;
        model.projectileImage = (item.Data.attacker != null && item.Data.attacker.AttackData != null && item.Data.attacker.AttackData.attackIcon != null) ? item.Data.attacker.AttackData.attackIcon.Icon : null;

        return model;
    }
}