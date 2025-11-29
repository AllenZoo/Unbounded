using UnityEngine;

[RequireComponent(typeof(CircleScaler), typeof(CircleColourChanger))]
public class AttackIndicatorInstance : MonoBehaviour
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
        scaler.SetCircleRadius(data.indicatorRadius);

        colourChanger.TransitionColour(
            data.startFillColour,
            data.endFillColour,
            data.transitionTime
        );
    }
}
