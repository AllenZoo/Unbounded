using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the application of all modifiers on a Item component.
/// </summary>
public class ItemModifierMediator : IUpgradeModifierVisitor
{
    private Item item;

    // Holds the stats of the item after modification.
    private StatComponent statsPostMod;

    public ItemModifierMediator(Item item)
    {
        this.item = item;
        statsPostMod = new StatComponent();
    }

    public StatComponent GetStatsAfterModification()
    {
        return statsPostMod;
    }


    private void ApplyModifiers(List<IUpgradeModifier> modifiers)
    {
        foreach (var modifier in modifiers)
        {
            modifier.Accept(this);
        }
    }

    // TODO: get stat modifiers.

    public virtual void Visit(StatModifier modifier) {
        
    }
    public virtual void Visit(DamageModifier modifier) { 
        
    }
    public virtual void Visit(TraitModifier modifier) { 
        
    }
}
