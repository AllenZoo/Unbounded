using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SerializableItemComponent
{
    public enum ComponentType
    {
        Attack,
        BaseStat,
        Upgrade,
        Upgrader,
        Equipment,
        // Add other types as needed
    }

    [HideLabel]
    public ComponentType type;

    [InlineProperty]
    [HideLabel]
    [SerializeReference]
    public IItemComponent component;

    public SerializableItemComponent()
    {
    }

    public SerializableItemComponent(ComponentType type, IItemComponent component)
    {
        this.type = type;
        this.component = component;
    }

    public void SetComponent(IItemComponent newComponent)
    {
        if (newComponent is ItemAttackContainerComponent)
            type = ComponentType.Attack;
        else if (newComponent is ItemUpgradeComponent)
            type = ComponentType.Upgrade;
        else if (newComponent is ItemBaseStatComponent)
            type = ComponentType.BaseStat;
        else if (newComponent is ItemUpgraderComponent)
            type = ComponentType.Upgrader;
        else if (newComponent is ItemEquipmentComponent)
            type = ComponentType.Equipment;

        // Add other types as needed

        component = newComponent;
    }

    public SerializableItemComponent DeepCopy()
    {
        var newComponent = new SerializableItemComponent();
        newComponent.SetComponent(DeepCopyComponent(component));
        return newComponent;
    }

    private IItemComponent DeepCopyComponent(IItemComponent component)
    {
        if (component is ItemBaseStatComponent baseStatComponent)
        {
            List<StatModifierEquipment> copy = baseStatComponent.statModifiers.Select(s => s.DeepCopy()).ToList();
            return new ItemBaseStatComponent(copy);
        }
        else if (component is ItemUpgradeComponent upgradeComponent)
        {
            List<StatModifierEquipment> copy = upgradeComponent.upgradeStatModifiers.Select(s => s.DeepCopy()).ToList();
            return new ItemUpgradeComponent(copy);
        }
        else if (component is ItemAttackContainerComponent attackComponent)
        {
            return new ItemAttackContainerComponent(attackComponent.attackerData);
        }
        else if (component is ItemUpgraderComponent upgraderComponent)
        {
            List<StatModifierEquipment> copy = upgraderComponent.modifiers.Select(s => s.DeepCopy()).ToList();
            return new ItemUpgraderComponent(copy, upgraderComponent.costPerItem);
        } 
        else if (component is ItemEquipmentComponent equipmentComponent)
        {
            return new ItemEquipmentComponent(equipmentComponent.equipmentType);
        }

        // Add more component types as needed
        Debug.LogError("Deep copy not implemented for component type: " + component.GetType());

        // If the component type is not recognized, return a shallow copy
        return component;
    }
}