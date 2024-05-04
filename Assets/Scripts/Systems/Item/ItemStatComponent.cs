using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemStatComponent : IItemComponent
{
    public List<IStatModifier> statModifiers = new List<IStatModifier>();
}
