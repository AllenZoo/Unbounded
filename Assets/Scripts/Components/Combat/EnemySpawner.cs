using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, ISpawner
{
    [SerializeField] private SpawnRates spawnRates;

    /// <summary>
    /// Spawn an enemy object at given position
    /// </summary>
    /// <param name="pos"></param>
    public void Spawn(Vector2 pos, GameObject spawn)
    {
        Instantiate(spawn, pos, Quaternion.identity);
    }

    /// <summary>
    /// Spawns multiple enemies around a given position
    /// </summary>
    /// <param name="centerPos"> center of position of spawned enemies</param>
    /// <param name="amount"> amount of enemies to spawn </param>
    public void Spawn(Vector2 centerPos, int amount)
    {

    }

    /// <summary>
    /// Given a spawn rate, return list of enemies to spawn.
    /// </summary>
    /// <param name="spawnRates"></param>
    /// <returns></returns>
    public List<GameObject> GetSpawnList(SpawnRates spawnRates)
    {
        List<GameObject> spawnList = new List<GameObject> ();
        foreach (SpawnRate spawnRate in spawnRates.data.spawnRates)
        {
            // Iterate through min spawn of each rate.
            for (int i = 0; i < spawnRate.minSpawn; i++)
            {
                // Add the spawn to the list.
            }
        }

        return null;
    }
    
}
