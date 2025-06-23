using UnityEngine;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

public class BossSelecter : MonoBehaviour
{
    [SerializeField] private List<BossSelectionEntry> bossScenes;

    /// <summary>
    /// Returns a random boss scene from the list.
    /// Returns null if the list is empty or unassigned.
    /// </summary>
    public BossSelectionEntry GetRandomBossScene()
    {
        if (bossScenes == null || bossScenes.Count == 0)
        {
            Debug.LogWarning("[BossSelecter] bossScenes list is empty or null.");
            return null;
        }

        int index = UnityEngine.Random.Range(0, bossScenes.Count);
        return bossScenes[index];
    }
}

[Serializable]
public class BossSelectionEntry
{
    [SerializeField, Required] private SceneField bossScene;
    [SerializeField, Required] private GameObject boss;
} 