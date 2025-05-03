using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the application of all modifiers on a Item component.
/// Stores the final modified state of each relevant weapon component.
/// Main entrypoint for other systems wanting to get the post modification state of item.
/// </summary>
public class ItemModifierMediator : IUpgradeModifierVisitor
{
    private Item item;

    // Holds the stats of the item after modification.
    private StatContainer statContainer;

    // TODO: for attack container trait modifiaction.
    // private AttackContainer attackContainer;

    private ItemBaseStatComponent baseStatComponent;

    public ItemModifierMediator(Item item)
    {
        this.item = item;


        baseStatComponent = item.GetComponent<ItemBaseStatComponent>();
        if (baseStatComponent != null)
        {
            statContainer = new StatContainer(baseStatComponent.BaseStats);
        }
    }

    public Optional<StatContainer> GetStatsAfterModification()
    {
        if (baseStatComponent == null)
        {
            if (Debug.isDebugBuild) Debug.LogError("Base stat component is null!");
            return new Optional<StatContainer>(null);
        }       

        ItemUpgradeComponent component = item.GetComponent<ItemUpgradeComponent>();
        if (component != null) {
            //Debug.Log("Adding upgrade modifiers");
            ClearModifiers();
            ApplyModifiers(component.GetUpgradeModifiers());
        }

        return new Optional<StatContainer>(statContainer);
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

    /// <summary>
    /// Clears all previously applied modifier.
    /// </summary>
    private void ClearModifiers()
    {
        statContainer.StatMediator.ClearModifiers();
    }

    public virtual void Visit(StatModifier modifier) {
        // Apply modifier!
        statContainer.StatMediator.AddModifier(modifier);
    }

    public virtual void Visit(DamageModifier modifier) { 
        
    }

    public virtual void Visit(TraitModifier modifier) { 
        
    }
}
