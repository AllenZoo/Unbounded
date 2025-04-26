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

    public StatModifierContainer DeepCopy()
    {
        StatModifierContainer copy = new StatModifierContainer
        {
            OperationType = OperationType,
            Stat = Stat,
            Value = Value,
        };
        return copy;
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

    public StatModifierEquipment DeepCopy()
    {
        StatModifierEquipment copy = new StatModifierEquipment
        {
            OperationType = OperationType,
            Stat = Stat,
            Value = Value,
        };
        return copy;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        StatModifierEquipment other = obj as StatModifierEquipment;
        return OperationType == other.OperationType && Stat == other.Stat && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(OperationType.GetHashCode(), Stat.GetHashCode(), Value.GetHashCode());
    }
}

public enum OperationType
{
    Add,
    Multiply,
}
