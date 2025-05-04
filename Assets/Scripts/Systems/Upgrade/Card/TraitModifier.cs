using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TraitModifier : IUpgradeModifier
{
    [field: SerializeField] public bool AddPiercing { get; private set; } = false;

    [field: SerializeField] public int NumAtksToAdd { get; private set; } = 0;

    public void Accept(IUpgradeModifierVisitor visitor)
    {
        visitor.Visit(this);
    }
}
