using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

// TODO: add some caching system so that we don't have to constantly clear and apply modifiers. We should cache stuff for better performance.

/// <summary>
/// Handles the application of all modifiers on a Item component.
/// Stores the final modified state of each relevant weapon component.
/// Main entrypoint for other systems wanting to get the post modification state of item.
/// </summary>
public class ItemModifierMediator : IUpgradeModifierVisitor
{
    /// <summary>
    /// Event invoked whenever a new modifier is added to item.
    /// </summary>
    public Action<Item> OnModifierChange;

    private CacheMediator<StatContainer, IUpgradeModifier> statCache;


    private Item item;
    private ItemBaseStatComponent baseStatComponent;
    private ItemUpgradeComponent upgradeComponent;
    private Attacker baseAttacker; // The base Attacker instance to create a copy of.
    private Attacker dynamicAttacker; // Accumlator of modifier visitor. Deep copy of base attacker.

    // Holds the stats of the item after modification. The accumulator passed in the modifier visitor.
    private StatContainer statContainer;

    // Accumulator for damage modifiers.
    private double percentageDamageIncrease = 0;

    public ItemModifierMediator(Item item)
    {
        this.item = item;


        baseStatComponent = item.GetComponent<ItemBaseStatComponent>();
        if (baseStatComponent != null)
        {
            statContainer = new StatContainer(baseStatComponent.BaseStats);
        }

        upgradeComponent = item.GetComponent<ItemUpgradeComponent>();
        if (upgradeComponent != null)
        {
            upgradeComponent.OnUpgradeModifierChange += () => OnModifierChange?.Invoke(item);
        }

        baseAttacker = item.IsEmpty() ? null : item?.Data?.attacker;

    }

    #region Mediator Query Functions
    public double GetPercentageDamageIncreaseTotal()
    {
        ClearModifiers(ModifierType.Damage);
        ApplyModifiers(upgradeComponent, ModifierType.Damage);
        return percentageDamageIncrease;
    }
    public Optional<StatContainer> GetStatsBeforeModification()
    {
        if (baseStatComponent != null)
        {
            return new Optional<StatContainer>(new StatContainer(baseStatComponent.BaseStats));
        } else {
            if (Debug.isDebugBuild) Debug.LogError("Base stat component is null!");
            return new Optional<StatContainer>(null);
        }
    }
    public Optional<StatContainer> GetStatsAfterModification()
    {
        if (baseStatComponent == null)
        {
            if (Debug.isDebugBuild) Debug.LogError("Base stat component is null!");
            return new Optional<StatContainer>(null);
        }

        ClearModifiers(ModifierType.Stat);
        ApplyModifiers(upgradeComponent, ModifierType.Stat);

        return new Optional<StatContainer>(statContainer);
    }
    public Attacker GetAttackerAfterModification()
    {
        ClearModifiers(ModifierType.Trait);
        ApplyModifiers(upgradeComponent, ModifierType.Trait);
        ApplyModifiers(upgradeComponent, ModifierType.Range);
        ApplyModifiers(upgradeComponent, ModifierType.ProjectileSpeed);
        return dynamicAttacker;
    }
    #endregion

    #region Modifier Application Helpers
    private enum ModifierType
    {
        Stat, // eg. + 1 ATK
        Damage, // eg. + 10% damage
        Trait, // eg. Add Weapon Piercing.
        Range, // eg. Range Increase
        ProjectileSpeed, // eg. Projectile Speed Increase
        All, // all of the above.
    }

    /// <summary>
    /// Applies the modifiers.
    /// </summary>
    /// <param name="modifiers"></param>
    private void ApplyModifiers(List<IUpgradeModifier> modifiers)
    {
        foreach (var modifier in modifiers)
        {
            modifier.Accept(this);
        }
    }

    private void ApplyModifiers(ItemUpgradeComponent component, ModifierType modifierType)
    {
        if (component == null)
        {
            Debug.Log("Failed to apply Modifiers for item with no ItemUpgradeComponent");
            return;
        }
        ApplyModifiers(GetModifiersOfType(component, modifierType));
    }

    private List<IUpgradeModifier> GetModifiersOfType(ItemUpgradeComponent component, ModifierType modifierType)
    {
        if (component == null)
        {
            Debug.Log("No upgrade component to extract upgrade modifiers from!");
            return new List<IUpgradeModifier>();
        }

        var fullList = component.GetUpgradeModifiers();

        return fullList.Where(m => modifierType switch
        {
            ModifierType.Stat => m is StatModifier,
            ModifierType.Damage => m is DamageModifier,
            ModifierType.Trait => m is TraitModifier,
            ModifierType.Range => m is RangeModifier,
            ModifierType.ProjectileSpeed => m is ProjectileSpeedModifier,
            ModifierType.All => true,
            _ => false
        }).ToList();

    }


    /// <summary>
    /// Clears all previously applied modifier.
    /// </summary>
    private void ClearModifiers(ModifierType modType)
    {
        switch (modType)
        {
            case ModifierType.Stat:
                statContainer?.StatMediator.ClearModifiers();
                break;
            case ModifierType.Damage:
                percentageDamageIncrease = 0;
                break;
            case ModifierType.Trait:
                if (baseAttacker != null) { 
                    // Clean up previously allocated attacker SOs
                    if (dynamicAttacker != null) { 
                        if (dynamicAttacker.AttackData != null)
                            UnityEngine.Object.Destroy(dynamicAttacker.AttackData);

                        if (dynamicAttacker.AttackerData != null)
                            UnityEngine.Object.Destroy(dynamicAttacker.AttackerData);
                    }

                    var attackerData = ScriptableObject.Instantiate(baseAttacker?.AttackerData);
                    var attackData = ScriptableObject.Instantiate(baseAttacker?.AttackData);
                    dynamicAttacker = new Attacker(attackerData, attackData);
                }
                break;
        }
    }
    #endregion

    #region Visiter Functions
    public virtual void Visit(StatModifier modifier) {
        // Apply modifier!
        statContainer.StatMediator.AddModifier(modifier);
    }

    public virtual void Visit(DamageModifier modifier) {
        // Apply Modifier
        percentageDamageIncrease += modifier.PercentageIncrease;
    }

    public virtual void Visit(TraitModifier modifier) { 
        // Apply Modifier
        if (modifier.AddPiercing)
        {
            dynamicAttacker.AttackData.isPiercing = true;
        }

        dynamicAttacker.AttackerData.numAttacks += modifier.NumAtksToAdd;
    }

    public virtual void Visit(RangeModifier modifier)
    {
        dynamicAttacker.AttackData.distance += modifier.RangeToAdd;
    }

    public virtual void Visit(ProjectileSpeedModifier modifier)
    {
        dynamicAttacker.AttackData.initialSpeed += modifier.ProjectileSpeedToAdd;
    }
    #endregion
}
