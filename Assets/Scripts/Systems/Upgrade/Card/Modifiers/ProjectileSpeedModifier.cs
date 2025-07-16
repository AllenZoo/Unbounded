using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileSpeedModifier : IUpgradeModifier
{
    [field: SerializeField] public float ProjectileSpeedToAdd { get; private set; } = 1f;

    public void Accept(IUpgradeModifierVisitor visitor)
    {
        visitor.Visit(this);
    }
}
