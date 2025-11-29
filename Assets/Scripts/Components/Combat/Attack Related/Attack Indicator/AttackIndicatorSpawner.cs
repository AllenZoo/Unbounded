using UnityEngine;

public class AttackIndicatorSpawner
{
    // TODO: spawns indicator at given location (update parameters, look to AttackSpawner for reference).
    public static CircleAttackIndicatorInstance SpawnIndicator(AttackIndicatorContext context, GameObject indicatorPfb)
    {
        // TODO: implement pool for indicators later, and add parent transform to this object.
        var go = GameObject.Instantiate(indicatorPfb, context.IndicatorSpawnPoint, Quaternion.identity);
        return go.GetComponent<CircleAttackIndicatorInstance>();
    }
}
