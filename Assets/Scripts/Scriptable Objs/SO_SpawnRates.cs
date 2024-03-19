using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnRates", menuName = "ScriptableObjs/SpawnRates", order = 1)]
public class SO_SpawnRates : ScriptableObject
{
    public List<SpawnRate> spawnRates = new List<SpawnRate>();
}

[System.Serializable]
public class SpawnRates
{
    public SO_SpawnRates data;
}

[System.Serializable]
public class SpawnRate
{
    public GameObject prefab;
    public float minSpawn;
    // public float maxSpawn;

    /// <summary>
    /// Measured in scale of 1. Rate is relative to other spawn rates.
    /// </summary>
    [Tooltip("Measured in scale of 1. Rate is relative to other spawn rates.")]
    public float spawnRate;
}


