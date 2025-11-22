using System;
using System.Collections.Generic;
using UnityEngine;


public interface IAttacker
{
    public void Attack(KeyCode keyCode, AttackSpawnInfo info, Transform attackerTransform, List<EntityType> targetTypes, float atkStat, double percentageDamageIncrease);
    public bool IsInitialized();
    public float GetCooldown();
    public float GetChargeUp();

    // TODO: figure out if there's a better way to pass in Data info through interface.

    // Fields required from this: numAttacks, wholeObj <- ItemModifierMediator + smt else.
    public AttackerData AttackerData { get; set; }

    // Fields required from this:
    // [rotOffset, distance, initialSpeed, wholeObj] <- AttackSpawner
    // [initialSpeed, distance, isPiercing] <- ItemDataConverter
    // [distance, initialSpeed, isPiercing, wholeObj] <- ItemModifierMediator.. for applying modifiers that change these attributes.
    public AttackData AttackData { get; set; }
}
