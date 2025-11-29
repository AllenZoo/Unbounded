using System;
using UnityEngine;

[CreateAssetMenu(fileName = "new MeteorAttackerData", menuName = "System/Combat/MeteorAttacker", order = 1)]
public class MeteorAttackerData : AttackerData
{
    [Tooltip("(min, max) radius of the meteor impact area")]
    public Vector2 meteorRadiusRange; // min and max radius of the meteor impact area

    [Tooltip("Vector containing the min and max time for the meteor to drop after spawning the indicator (aka. how long until the indicator fully transitions). (min, max)")]
    public Vector2 dropTimeRange; // min and max time for meteor to drop

    [Tooltip("Vector containing the min and max error range for the meteor spawn position around the target point. (min, max)")]
    public Vector2 errorRange; // min and max error for meteor spawn position
}
