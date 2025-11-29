using UnityEngine;


[CreateAssetMenu(fileName = "new Circle Attack Indicator", menuName = "System/Combat/Indicator/Circle", order = 1)]
public class CircleAttackIndicatorData : AttackIndicatorData
{
    [Tooltip("min and max radius of the circle indicator.")]
    public Vector2 radiusRange = new Vector2(1f, 1f);
}
