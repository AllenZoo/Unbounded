using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatModifierContainer
{
    StatModifier GetModifier();
}

[Serializable]
public class StatModifierContainer : IStatModifierContainer
{
    public OperationType OperationType;
    public Stat Stat;
    public int Value;
    private StatModifier modifier;
    
    public StatModifier GetModifier()
    {
        if (modifier == null)
        {
            modifier = StatModifierFactory.Create(OperationType, Stat, Value, -1);
        }
        return modifier;
    }
}

[Serializable]
public class StatModifierEquipment : IStatModifierContainer
{
    public OperationType OperationType;
    public Stat Stat;
    public int Value;
    private StatModifier modifier;
    
    public StatModifier GetModifier()
    {
        if (modifier == null)
        {
            modifier = StatModifierFactory.Create(OperationType, Stat, Value, -1);
        }

        return modifier;
    }
}

public enum OperationType
{
    Add,
    Multiply,
}
