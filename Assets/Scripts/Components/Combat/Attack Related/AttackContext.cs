using System.Collections.Generic;
using UnityEngine;

public class AttackContext
{
    // TODO: remake this as TargetTransform
    public AttackSpawnInfo SpawnInfo { get; set; }
    public AttackerComponent AttackerComponent { get; set; }
    public Transform AttackerTransform { get; set; }
    public List<EntityType> TargetTypes { get; set; }
    public float AtkStat { get; set; }
    public double PercentageDamageIncrease { get; set; }

    // Optional: Constructor for convenience
    public AttackContext(
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
