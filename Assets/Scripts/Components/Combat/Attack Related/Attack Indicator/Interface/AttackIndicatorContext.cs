using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Class that holds context information for spawning an attack indicator. (essentially just encapsulate function parameters)
/// 
/// Passed through entry point of IAttackIndicator.Indicate(...)
/// </summary>
public class AttackIndicatorContext
{
    public Vector3 IndicatorSpawnPoint;

    // Radius override for when setting up the radius dynamically.
    public float StartRadius;
    public float IndicatorRadius;
    public bool OverrideRadius = false;

    // Optional: Radius growth parameters
    public float RadiusGrowthTime = 0f;
    public bool GrowRadius = false;

    // Optional: Constructor for convenience
    public AttackIndicatorContext(Vector3 indicatorSpawnPoint, float startRadius = 0, float indicatorRadius = 0, bool overrideRadius = false, float radiusGrowthTime = 0, bool growRadius = false)
    {
        this.IndicatorSpawnPoint = indicatorSpawnPoint;
        StartRadius = startRadius;
        IndicatorRadius = indicatorRadius;
        OverrideRadius = overrideRadius;
        RadiusGrowthTime = radiusGrowthTime;
        GrowRadius = growRadius;
    }
}
