using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Information passed from AttackerComponent to AttackSpawner.
/// </summary>
public class AttackerContext
{
    public AttackSpawnInfo AttackSpawnInfo { get; set; }
    public AttackerComponent AttackerComponent { get; set; }
    public Transform AttackerTransform { get; set; }
    public double PercentageDamageIncrease { get; set; }
    public float AtkStat { get; set; }
    public float DexStat { get; set; }

    public float duration; // Duration before attack disappears.

    public AttackerContext() { }

    public AttackerContext(
        AttackSpawnInfo spawnInfo,
        AttackerComponent attackerComponent,
        float atkStat,
        float dexStat)
    {
        AttackSpawnInfo = spawnInfo;
        AttackerComponent = attackerComponent;

        AttackerTransform = attackerComponent.transform;
        PercentageDamageIncrease = attackerComponent.PercentageDamageIncrease;

        AtkStat = atkStat;
        DexStat = dexStat;
    }
}

[Serializable]
public class AttackDamageModifiers
{

    /// <summary>
    /// The the atk stat attached to Attack. Boosts the base damage of said attack.
    /// Generally the cumulation of weapon stats + player stats after modifiers applied for each.
    /// </summary>
    [ShowInInspector]
    public float AtkStat { get; set; }

    /// <summary>
    /// Damage modifier to apply to final calculated damage.
    /// For example after Attack.Damage - Damageable.Defense = TrueDamage
    /// We apply % modifier to TrueDamage: TrueDamage + TrueDamage * % modifier.
    /// </summary>
    [ShowInInspector]
    public double PercentageDamageIncrease { get; set; }

    public AttackDamageModifiers() { 
        AtkStat = 0;
        PercentageDamageIncrease = 0;
    }

    public AttackDamageModifiers(float atkStat, double percentageDamageIncrease)
    {
        AtkStat = atkStat;
        PercentageDamageIncrease = percentageDamageIncrease;
    }

}