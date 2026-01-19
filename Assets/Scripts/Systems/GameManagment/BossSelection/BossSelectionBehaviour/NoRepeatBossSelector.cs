using System.Linq;
using UnityEngine;

public class NoRepeatBossSelector : IBossSelector
{
    public BossListEntryData SelectBoss(BossSelectionContext selectionContext)
    {
        var pool = selectionContext.pool.bossList.ToList();
        var history = selectionContext.history;


        var available = pool;
        if (available.Count <= 1) return available.Count > 0 ? available[0] : null;

        BossListEntryData selected;
        do
        {
            selected = available[UnityEngine.Random.Range(0, available.Count)];
        } while (selected == history.LastBoss);

        return selected;
    }
}
