using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DamageModifier : IUpgradeModifier
{
    [SerializeField, Tooltip("In %. Apply after base damage calculation (ATK minus DEF)")]
    private double percentageIncrease = 10;

    public void Accept(IUpgradeModifierVisitor visitor)
    {
        visitor.Visit(this);
    }
}
