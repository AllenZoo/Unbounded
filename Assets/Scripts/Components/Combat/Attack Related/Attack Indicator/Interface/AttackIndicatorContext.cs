using System.Collections.Generic;
using UnityEngine;

public class AttackIndicatorContext
{
    // TODO: modify as needed to fit spawning indicator requirements
    public AttackSpawnInfo SpawnInfo { get; set; }
    public AttackerComponent AttackerComponent { get; set; }
    public Transform AttackerTransform { get; set; }
    public List<EntityType> TargetTypes { get; set; }
    public float AtkStat { get; set; }
    public double PercentageDamageIncrease { get; set; }

    // Optional: Constructor for convenience
    public AttackIndicatorContext(
        AttackSpawnInfo spawnInfo,
        AttackerComponent attackerComponent,
        Transform attackerTransform,
        List<EntityType> targetTypes,
        float atkStat,
        double percentageDamageIncrease)
    {
        SpawnInfo = spawnInfo;
        AttackerComponent = attackerComponent;
        AttackerTransform = attackerTransform;
        TargetTypes = targetTypes;
        AtkStat = atkStat;
        PercentageDamageIncrease = percentageDamageIncrease;
    }
}
