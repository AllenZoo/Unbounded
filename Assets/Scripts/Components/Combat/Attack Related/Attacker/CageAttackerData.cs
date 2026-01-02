using UnityEngine;


[CreateAssetMenu(fileName = "new CageAttackerData", menuName = "System/Combat/CageAttacker", order = 1)]
public class CageAttackerData : AttackerData
{
    [Header("Cage Attacker Data Section")]
    [Tooltip("Num of objects in cage")]
    public float CageAttackDensity;

    [Tooltip("Max Radius that cage expands to")]
    public float CageOuterRadius; // min and max radius of the meteor impact area

    [Tooltip("Min Radius that cage shrinks to")]
    public float CageInnerRadius;

    [Tooltip("Rotational Speed of the Cage (full circle/s)")]
    public float RotationalSpeed;

    [Tooltip("Time in seconds for cage to grow, shrink and then grow back to full outer radius again.")]
    public float CycleTime;
}
