using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableItemComponent
{
    public enum ComponentType
    {
        Attack,
        Upgrade,
        BaseStat,
        // Add other component types here
    }

    public ComponentType type;

    [SerializeReference]
    public IItemComponent component;

    public void SetComponent(IItemComponent newComponent)
    {
        if (newComponent is ItemAttackContainerComponent)
            type = ComponentType.Attack;
        else if (newComponent is ItemUpgradeComponent)
            type = ComponentType.Upgrade;
        else if (newComponent is ItemBaseStatComponent)
            type = ComponentType.BaseStat;
        // Add other types as needed

        component = newComponent;
    }
}