using UnityEngine;

[RequireComponent(typeof(CircleScaler), typeof(CircleColourChanger))]
public class CircleAttackIndicatorInstance : MonoBehaviour
{
    private CircleScaler scaler;
    private CircleColourChanger colourChanger;

    private void Awake()
    {
        scaler = GetComponent<CircleScaler>();
        colourChanger = GetComponent<CircleColourChanger>();
    }

    public void Setup(CircleAttackIndicatorData data)
    {
        // Get meteor size (given from random range)
        var indicatorRadius = UnityEngine.Random.Range(
            data.radiusRange.x,
            data.radiusRange.y);

        scaler.SetCircleRadius(indicatorRadius);

        colourChanger.TransitionColour(
            data.startFillColour,
            data.endFillColour,
            data.transitionTime
        );
    }

    /// <summary>
    /// Alternative set up where we manually change the radius
    /// </summary>
    /// <param name="data"></param>
    /// <param name="radius"></param>
    public void Setup(CircleAttackIndicatorData data, float radius)
    {
        scaler.SetCircleRadius(radius);

        colourChanger.TransitionColour(
            data.startFillColour,
            data.endFillColour,
            data.transitionTime
        );
    }
}
