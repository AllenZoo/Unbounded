using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpgradeModifierVisitor
{
    void Visit(StatModifier modifier);
    void Visit(DamageModifier modifier);
    void Visit(TraitModifier modifier);
}

