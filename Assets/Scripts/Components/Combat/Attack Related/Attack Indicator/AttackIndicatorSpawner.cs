using UnityEngine;

public class AttackIndicatorSpawner
{
    // TODO: spawns indicator at given location (update parameters, look to AttackSpawner for reference).
    public static void SpawnIndicator(Transform parent, GameObject indicatorPfb)
    {
        var go = GameObject.Instantiate(indicatorPfb, parent.position, Quaternion.identity, parent);
    }
}
