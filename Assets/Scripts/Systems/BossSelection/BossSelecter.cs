using UnityEngine;
using System.Collections.Generic;

public class BossSelecter : MonoBehaviour
{
    [SerializeField] private List<SceneField> bossScenes;

    /// <summary>
    /// Returns a random boss scene from the list.
    /// Returns null if the list is empty or unassigned.
    /// </summary>
    public SceneField GetRandomBossScene()
    {
        if (bossScenes == null || bossScenes.Count == 0)
        {
            Debug.LogWarning("[BossSelecter] bossScenes list is empty or null.");
            return null;
        }

        int index = Random.Range(0, bossScenes.Count);
        return bossScenes[index];
    }
}
