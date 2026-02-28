using System.Linq;
using UnityEngine;

public class RandomBossSelector : IBossSelector
{
    public BossListEntryData SelectBoss(BossSelectionContext selectionContext)
    {
        var pool = selectionContext.pool.bossList.ToList();

        if (pool.Count == 0) return null;
        return pool[UnityEngine.Random.Range(0, pool.Count)];
    }
}
