using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float maxSpawn;
    public float spawnRate;
}


