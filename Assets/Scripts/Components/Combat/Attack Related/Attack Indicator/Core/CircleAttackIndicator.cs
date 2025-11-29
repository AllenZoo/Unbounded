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

        //indicator.GetComponent<AttackIndicatorComponent>().

        // Set scale of indicator to match radius.
        indicator.GetComponent<CircleScaler>().SetCircleRadius(data.indicatorRadius);

        // Start Transition of indicator from transparent to opaque over duration.
        indicator.GetComponent<CircleColourChanger>().TransitionColour(data.startFillColour, data.endFillColour, data.transitionTime);
    }

    public void Test(GameObject indicator)
    {
        // Set scale of indicator to match radius.
        indicator.GetComponent<CircleScaler>().SetCircleRadius(data.indicatorRadius);

        // Start Transition of indicator from transparent to opaque over duration.
        indicator.GetComponent<CircleColourChanger>().TransitionColour(data.startFillColour, data.endFillColour, data.transitionTime);
    }
}
