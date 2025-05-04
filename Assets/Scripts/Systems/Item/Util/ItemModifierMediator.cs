using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// TODO: add some caching system so that we don't have to constantly clear and apply modifiers. We should cache stuff for better performance.

/// <summary>
/// Handles the application of all modifiers on a Item component.
/// Stores the final modified state of each relevant weapon component.
/// Main entrypoint for other systems wanting to get the post modification state of item.
/// </summary>
public class ItemModifierMediator : IUpgradeModifierVisitor
{
    // TODO: make StatComponent subscribe to this.
    //       handles the case of player upgrading the weapon but not requipping it.
    public Action<Item> OnModifierChange;


    private Item item;
    private ItemBaseStatComponent baseStatComponent;
    private ItemUpgradeComponent upgradeComponent;
    private Attacker baseAttacker; // The base Attacker instance to create a copy of.
    private Attacker dynamicAttacker; // Accumlator of modifier visitor.

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

        baseAttacker = item?.data?.attacker;
    }


    public double GetPercentageDamageIncreaseTotal()
    {
        ClearModifiers();
        ApplyModifiers(upgradeComponent);
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

        ClearModifiers();
        ApplyModifiers(upgradeComponent);

        return new Optional<StatContainer>(statContainer);
    }
    public Attacker GetAttackerAfterModification()
    {
        ClearModifiers();
        ApplyModifiers(upgradeComponent);
        return dynamicAttacker;
    }

    // TODO: for letting StatComponent add and remove the same stat modifiers from adding item.
    // maybe doesn't work considering how upgrades will make previous StatModifier irrelevant.
    public StatModifier PackageStat(Stat stat)
    {
        return null;
    }

    #region Modifier Application Helpers
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

    private void ApplyModifiers(ItemUpgradeComponent component)
    {
        if (component == null)
        {
            Debug.Log("Failed to apply Modifiers for item with no ItemUpgradeComponent");
            return;
        }
        ApplyModifiers(component.GetUpgradeModifiers());
    }

    /// <summary>
    /// Clears all previously applied modifier.
    /// </summary>
    private void ClearModifiers()
    {
        statContainer.StatMediator.ClearModifiers();
        percentageDamageIncrease = 0;

        var attackerData = ScriptableObject.Instantiate(baseAttacker?.AttackerData);
        var attackData = ScriptableObject.Instantiate(baseAttacker?.AttackData);
        dynamicAttacker = new Attacker(attackerData, attackData);
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
    #endregion
}
