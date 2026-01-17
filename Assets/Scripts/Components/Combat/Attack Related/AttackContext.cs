using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Information passed from AttackerComponent to AttackSpawner.
/// </summary>
public class AttackContext
{
    public AttackSpawnInfo AttackSpawnInfo { get; set; }
    public AttackerComponent AttackerComponent { get; set; }
    public Transform AttackerTransform { get; set; }
    public double PercentageDamageIncrease { get; set; }
    public float AtkStat { get; set; }

    public float duration; // Duration before attack disappears.

    public AttackContext() { }

    public AttackContext(
        AttackSpawnInfo spawnInfo,
        AttackerComponent attackerComponent,
        float atkStat)
    {
        AttackSpawnInfo = spawnInfo;
        AttackerComponent = attackerComponent;

        AttackerTransform = attackerComponent.transform;
        PercentageDamageIncrease = attackerComponent.PercentageDamageIncrease;

        AtkStat = atkStat;
    }
}
