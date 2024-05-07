using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemUpgradeComponent : IItemComponent
{
    public List<StatModifierEquipment> upgradeStatModifiers = new List<StatModifierEquipment>();
}
