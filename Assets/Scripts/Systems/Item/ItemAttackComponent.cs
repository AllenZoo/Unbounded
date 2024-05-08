using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ItemAttackComponent : IItemComponent
{
    public SO_Attacker attackerData;

    public ItemAttackComponent(SO_Attacker attackerData)
    {
        this.attackerData = attackerData;
    }
}

public interface IAttackItemBehaviour
{
    ItemAttackComponent GetItemAttackComponent();
}

/// <summary>
/// For items with standard attack behaviour.
/// </summary>
public class StandardAttackItemBehaviour: IAttackItemBehaviour
{
    private ItemAttackComponent attackComponent;
    public StandardAttackItemBehaviour(ItemAttackComponent attackComponent)
    {
        this.attackComponent = attackComponent;
    }
    public ItemAttackComponent GetItemAttackComponent()
    {
        return attackComponent;
    }
}

/// <summary>
/// For items with no attack behaviour.
/// </summary>
public class NoAttackItemBehaviour: IAttackItemBehaviour
{
    public ItemAttackComponent GetItemAttackComponent() {
        return null;
    }
}

