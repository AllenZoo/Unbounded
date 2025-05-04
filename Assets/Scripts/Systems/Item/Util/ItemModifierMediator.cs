using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// Handles the application of all modifiers on a Item component.
/// Stores the final modified state of each relevant weapon component.
/// Main entrypoint for other systems wanting to get the post modification state of item.
/// </summary>
public class ItemModifierMediator : IUpgradeModifierVisitor
{
    // TODO: make StatComponent subscribe to this.
    //       handles the case of player upgrading the weapon but not requipping it.
    public Action OnModifierChange;


    private Item item;
    private ItemBaseStatComponent baseStatComponent;
    private ItemUpgradeComponent upgradeComponent;

    // Holds the stats of the item after modification. The accumulator passed in the modifier visitor.
    private StatContainer statContainer;

    

    // Accumulator for damage modifiers.
    private double percentageDamageIncrease = 0;

    // TODO: for attack container trait modifiaction.
    // private AttackContainer attackContainer;

    public ItemModifierMediator(Item item)
    {
        this.item = item;


        baseStatComponent = item.GetComponent<ItemBaseStatComponent>();
        if (baseStatComponent != null)
        {
            statContainer = new StatContainer(baseStatComponent.BaseStats);
        }

        upgradeComponent = item.GetComponent<ItemUpgradeComponent>();
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


    #region Helpers
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
    }
    #endregion

    #region Visiter Functions
    public virtual void Visit(StatModifier modifier) {
        // Apply modifier!
        statContainer.StatMediator.AddModifier(modifier);
    }

    public virtual void Visit(DamageModifier modifier) {
        percentageDamageIncrease += modifier.PercentageIncrease;
    }

    public virtual void Visit(TraitModifier modifier) { 
        
    }
    #endregion
}
