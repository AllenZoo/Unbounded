using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Class that holds context information for spawning an attack indicator. (essentially just encapsulate function parameters)
/// </summary>
public class AttackIndicatorContext
{
    // TODO: modify as needed to fit spawning indicator requirements
    public Vector3 IndicatorSpawnPoint;

    // Radius override for when setting up the radius dynamically.
    public float IndicatorRadius;
    public bool OverrideRadius = false;

    // Optional: Constructor for convenience
    public AttackIndicatorContext(Vector3 indicatorSpawnPoint, float indicatorRadius = 0, bool overrideRadius = false)
    {
        this.IndicatorSpawnPoint = indicatorSpawnPoint;
        IndicatorRadius = indicatorRadius;
        OverrideRadius = overrideRadius;
    }
}
