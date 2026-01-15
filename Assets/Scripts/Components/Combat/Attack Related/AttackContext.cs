using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Information passed from AttackerComponent to AttackSpawner.
/// </summary>
public class AttackContext
{
    public AttackSpawnInfo AttackSpawnInfo { get; set; }
    public AttackerComponent AttackerComponent { get; set; }
    public Transform AttackerTransform => AttackerComponent.transform;
    public double PercentageDamageIncrease => AttackerComponent.PercentageDamageIncrease;
    public float AtkStat { get; set; }

    public float duration; // Duration before attack disappears.

    public AttackContext(
        AttackSpawnInfo spawnInfo,
        AttackerComponent attackerComponent,
        float atkStat)
    {
        AttackSpawnInfo = spawnInfo;
        AttackerComponent = attackerComponent;
        AtkStat = atkStat;
    }
}
