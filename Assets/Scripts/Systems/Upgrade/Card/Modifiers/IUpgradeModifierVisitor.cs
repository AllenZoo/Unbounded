using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IUpgradeModifierVisitor
{
    public virtual void Visit(StatModifier modifier) { }
    public virtual void Visit(DamageModifier modifier) { }
    public virtual void Visit(TraitModifier modifier) { }
    public virtual void Visit(RangeModifier modifier) { }
    public virtual void Visit(ProjectileSpeedModifier modifier) { }
}

