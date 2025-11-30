using System;
using UnityEngine;

[Serializable]
public class CircleAttackIndicator : IAttackIndicator
{
    public AttackIndicatorData Data { get { return data; } set { data = (CircleAttackIndicatorData) value; } }
    [SerializeField] private CircleAttackIndicatorData data;

    public CircleAttackIndicator() { }


    public void Indicate(AttackIndicatorContext context)
    {
        // Spawn indicator object at position with radius.
        var indicator = AttackIndicatorSpawner.SpawnIndicator(context, data.attackIndicatorPfb);

        if (context.OverrideRadius)
        {
            indicator.Setup(data, context.IndicatorRadius);
        } else
        {
            indicator.Setup(data);
        }
    }
}
