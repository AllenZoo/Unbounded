using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IStatModifierApplicationOrder
{
    /// <summary>
    /// Calculates and returns the final value of stat after applying all modifiers in a certain order. (depending on concrete class)
    /// </summary>
    /// <param name="statMods"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    float Apply(IEnumerable<StatModifier> statMods, float baseValue);
}

/// <summary>
/// Stat modifier application order that applies modifiers in-order as follows:
///    1. Additive modifiers
///    2. Multiplicative modifiers
/// </summary>
public class NormalStatModifierOrder : IStatModifierApplicationOrder
{
    public float Apply(IEnumerable<StatModifier> statMods, float baseValue)
    {
        var allModifiers = statMods.ToList();
        
        foreach (StatModifier statMod in allModifiers.Where(statMod => statMod.operation is AddOperation))
        {
            baseValue = statMod.operation.Calculate(baseValue);
        }

        foreach (StatModifier statMod in allModifiers.Where(statMod => statMod.operation is MultiplyOperation))
        {
            baseValue = statMod.operation.Calculate(baseValue);
        }

        return baseValue;
    }
}
