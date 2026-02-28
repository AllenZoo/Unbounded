using DG.Tweening;
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

        colourChanger
            .TransitionColour(data.startFillColour, data.endFillColour, data.transitionTime)
            .OnComplete(() => Destroy(gameObject, data.delayUntilDestroy)); // destroy after transition
    }

    /// <summary>
    /// Alternative set up where we manually change the radius
    /// </summary>
    /// <param name="data"></param>
    /// <param name="radius"></param>
    public void Setup(CircleAttackIndicatorData data, float radius)
    {
        scaler.SetCircleRadius(radius);

        colourChanger
            .TransitionColour(data.startFillColour, data.endFillColour, data.transitionTime)
            .OnComplete(() => Destroy(gameObject, data.delayUntilDestroy)); // destroy after transition
    }


    /// <summary>
    /// Alternative set up where we manually change the radius and there is a transition time to get to that radius
    /// </summary>
    /// <param name="data"></param>
    /// <param name="radius"></param>
    public void Setup(CircleAttackIndicatorData data, float startRadius, float endRadius, float transitionTime)
    {
        scaler.TransitionCircleRadius(startRadius, endRadius, transitionTime);

        // Note: we aren't using data.transitionTime, b/c we want to match the radius transition with the colour transition.
        colourChanger
            .TransitionColour(data.startFillColour, data.endFillColour, transitionTime)
            .OnComplete(() => Destroy(gameObject, data.delayUntilDestroy)); // destroy after transition
    }
}
