using System;
using System.Collections.Generic;
using System.Linq;

/**
 * For items that have base stats.
 */
public class ItemBaseStatComponent : IItemComponent
{
    public List<StatModifierEquipment> statModifiers = new List<StatModifierEquipment>();

    public ItemBaseStatComponent()
    {
        statModifiers = new List<StatModifierEquipment>();
    }

    public ItemBaseStatComponent(List<StatModifierEquipment> toBeCloned)
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

        ItemBaseStatComponent other = obj as ItemBaseStatComponent;
        bool isEqual = statModifiers.All(other.statModifiers.Contains) && statModifiers.Count == other.statModifiers.Count;
        return isEqual;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(statModifiers.GetHashCode());
    }
}
