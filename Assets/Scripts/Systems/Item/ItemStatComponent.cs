using System;
using System.Collections;
using System.Collections.Generic;
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
}
