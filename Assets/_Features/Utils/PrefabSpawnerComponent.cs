using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawnerComponent : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private List<PrefabSpawnerEntry> spawnList;

    public void SpawnObjects()
    {
        foreach (var entry in spawnList)
        {
            if (entry.spawnPrefab == null)
            {
                Debug.LogWarning("Spawn entry has no prefab assigned!");
                continue;
            }

            // Instantiate at the correct world position
            Vector3 worldSpawnPos = spawnTransform.TransformPoint(entry.spawnOffset);
            Quaternion worldRotation = spawnTransform.rotation;

            GameObject newPfb = Instantiate(entry.spawnPrefab, worldSpawnPos, worldRotation);

            // Optional: Unparent to make it part of the scene (not under spawnTransform)
            newPfb.transform.SetParent(null, true); // 'true' = keep world position
        }
    }

}



[Serializable]
public struct PrefabSpawnerEntry
{
    public Vector3 spawnOffset;
    public GameObject spawnPrefab;
}
