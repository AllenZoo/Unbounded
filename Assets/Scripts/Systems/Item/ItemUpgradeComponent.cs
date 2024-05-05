using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemUpgradeComponent : IItemComponent
{
    public List<StatModifier> upgradeStatModifiers = new List<StatModifier>();
}
