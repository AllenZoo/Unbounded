using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface IBossSelector
{
    BossListEntryData SelectBoss(BossSelectionContext bsc);
}

public class BossSelectionContext
{
    public BossPoolData pool;
    public BossSelectionHistory history;
}

// Simple state container
public class BossSelectionHistory
{
    public List<BossListEntryData> PreviousBosses = new();
    public BossListEntryData LastBoss => PreviousBosses.Count > 0 ? PreviousBosses[^1] : null;
}