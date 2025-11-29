using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data Class that holds context information for spawning an attack indicator. (essentially just encapsulate function parameters)
/// </summary>
public class AttackIndicatorContext
{
    // TODO: modify as needed to fit spawning indicator requirements
    public Vector3 IndicatorSpawnPoint;

    // Optional: Constructor for convenience
    public AttackIndicatorContext(Vector3 indicatorSpawnPoint)
    {
        this.IndicatorSpawnPoint = indicatorSpawnPoint;
    }
}
