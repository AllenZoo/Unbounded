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

public interface IEquipItemBehaviour
{
    bool Equip(StatComponent s);
    ItemStatComponent GetItemStatComponent();
}

/// <summary>
/// For items with standard equip behaviour.
/// </summary>
public class StandardEquipItemBehaviour : IEquipItemBehaviour
{
    private ItemStatComponent statComponent;
    public StandardEquipItemBehaviour(ItemStatComponent statComponent)
    {
        this.statComponent = statComponent;
    }
    public bool Equip(StatComponent s)
    {
        if (s == null)
        {
            return false;
        }
        foreach (var statModifier in statComponent.statModifiers)
        {
            s.statMediator.AddModifier(statModifier.GetModifier());
        }
        return true;
    }
    public ItemStatComponent GetItemStatComponent()
    {
        return statComponent;
    }
}

/// <summary>
/// For items with no equip behaviour.
/// </summary>
public class NoEquipItemBehaviour : IEquipItemBehaviour
{
    public bool Equip(StatComponent s)
    {
        return false;
    }
    public ItemStatComponent GetItemStatComponent()
    {
        return null;
    }
}