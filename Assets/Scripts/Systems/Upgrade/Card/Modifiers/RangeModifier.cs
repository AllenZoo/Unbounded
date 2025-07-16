using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RangeModifier : IUpgradeModifier
{
    [field: SerializeField] public float RangeToAdd { get; private set; } = 1f;

    public void Accept(IUpgradeModifierVisitor visitor)
    {
        visitor.Visit(this);
    }
}
