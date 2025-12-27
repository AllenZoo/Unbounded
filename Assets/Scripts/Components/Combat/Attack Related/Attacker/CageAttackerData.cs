using UnityEngine;


[CreateAssetMenu(fileName = "new CageAttackerData", menuName = "System/Combat/CageAttacker", order = 1)]
public class CageAttackerData : AttackerData
{
    [Tooltip("Num of objects in cage")]
    public float CageAttackDensity;

    [Tooltip("Max Radius that cage expands to")]
    public float CageOuterRadius; // min and max radius of the meteor impact area

    [Tooltip("Min Radius that cage shrinks to")]
    public float CageInnerRadius;

    [Tooltip("Timein seconds for cage to grow, shrink and then grow back to full outer radius again.")]
    public float CycleTime;
}
