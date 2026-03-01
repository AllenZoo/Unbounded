using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheMediator<TAccumulator, TModifier>
{
    private TAccumulator accumulator;
    private bool isDirty = true;

    private readonly Func<TAccumulator> createAccumulator;
    private readonly Action<TAccumulator> clearAccumulator;
    private readonly Action<TModifier, TAccumulator> applyModifier;

    private readonly Func<List<TModifier>> getModifiers;

    public CacheMediator(
        Func<TAccumulator> createAccumulator,
        Action<TAccumulator> clearAccumulator,
        Action<TModifier, TAccumulator> applyModifier,
        Func<List<TModifier>> getModifiers)
    {
        this.createAccumulator = createAccumulator;
        this.clearAccumulator = clearAccumulator;
        this.applyModifier = applyModifier;
        this.getModifiers = getModifiers;
    }

    public void Invalidate() => isDirty = true;

    public TAccumulator Get()
    {
        if (isDirty || accumulator == null)
        {
            accumulator = createAccumulator();
            clearAccumulator(accumulator);

            foreach (var mod in getModifiers())
                applyModifier(mod, accumulator);

            isDirty = false;
        }

        return accumulator;
    }
}
