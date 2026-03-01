using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new BossPool", menuName = "System/GameManagement/BossSelection/BossPool", order = 1)]
public class BossPoolData : SerializedScriptableObject
{
    //[OdinSerialize]
    [Title("Boss Pool")]
    [ListDrawerSettings(ShowIndexLabels = true)]
    [ValidateInput(nameof(NoDuplicates), "Duplicate cards are not allowed")]
    public List<BossListEntryData> bossList = new();

    private bool NoDuplicates(List<BossListEntryData> list)
    {
        return list.Count == new HashSet<BossListEntryData>(list).Count;
    }
}
