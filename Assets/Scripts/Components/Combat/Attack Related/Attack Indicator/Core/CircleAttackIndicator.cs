using System;
using UnityEngine;

[Serializable]
public class CircleAttackIndicator : IAttackIndicator
{
    public CircleAttackIndicatorData Data { get { return data; } set { data = value; } }
    [SerializeField] private CircleAttackIndicatorData data;

    public CircleAttackIndicator() { }


    public void Indicate(AttackIndicatorContext context)
    {
        // Spawn indicator object at position with radius.
        var indicator = AttackIndicatorSpawner.SpawnIndicator(context.IndicatorSpawnPoint, data.attackIndicatorPfb);
        indicator.Setup(data);
    }
}
