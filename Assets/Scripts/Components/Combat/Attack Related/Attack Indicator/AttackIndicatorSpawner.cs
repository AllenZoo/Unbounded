using UnityEngine;

public class AttackIndicatorSpawner
{
    // TODO: spawns indicator at given location (update parameters, look to AttackSpawner for reference).
    public static AttackIndicatorInstance SpawnIndicator(Vector3 spawnPos, GameObject indicatorPfb)
    {
        // TODO: implement pool for indicators later, and add parent transform to this object.
        var go = GameObject.Instantiate(indicatorPfb, spawnPos, Quaternion.identity);
        return go.GetComponent<AttackIndicatorInstance>();
    }
}
