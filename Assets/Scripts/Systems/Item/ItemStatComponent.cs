using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ItemStatComponent : IItemComponent
{
    public List<StatModifierEquipment> statModifiers = new List<StatModifierEquipment>();

    public ItemStatComponent()
    {
        statModifiers = new List<StatModifierEquipment>();
    }

    public ItemStatComponent(List<StatModifierEquipment> toBeCloned)
    {
        this.statModifiers = new List<StatModifierEquipment>();
        foreach (var statModifier in toBeCloned)
        {
            this.statModifiers.Add(statModifier.DeepCopy());
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        // Check if lists match each other regardless of order.

        ItemStatComponent other = obj as ItemStatComponent;
        bool isEqual = statModifiers.All(other.statModifiers.Contains) && statModifiers.Count == other.statModifiers.Count;
        return isEqual;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(statModifiers.GetHashCode());
    }
}
