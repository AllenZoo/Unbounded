using UnityEngine;
using Sirenix.OdinInspector;

public class BossSelectorComponent : SerializedMonoBehaviour
{
    [Title("Configuration")]
    [Required, SerializeField] private BossPoolData bossPool;
    [Required, SerializeField] private IBossSelector selectorStrategy;

    [Title("Runtime State")]
    [ReadOnly, ShowInInspector]
    private BossSelectionHistory history = new BossSelectionHistory();

    /// <summary>
    /// Logic to select a boss and track history.
    /// </summary>
    public BossListEntryData GetNextBoss()
    {
        if (bossPool == null || selectorStrategy == null)
        {
            Debug.LogError($"[BossSelector] Missing configuration on {gameObject.name}");
            return null;
        }

        var bsc = new BossSelectionContext
        {
            pool = bossPool,
            history = history
        };

        BossListEntryData selection = selectorStrategy.SelectBoss(bsc);

        if (selection != null)
        {
            RecordSelection(selection);
        }

        return selection;
    }

    private void RecordSelection(BossListEntryData boss)
    {
        history.PreviousBosses.Add(boss);
        // Optional: Limit history size to prevent memory leaks in endless games
        if (history.PreviousBosses.Count > 10) history.PreviousBosses.RemoveAt(0);
    }

    // Context menu for testing in editor
    [Button, GUIColor(0, 1, 0)]
    private void TestSelection()
    {
        var result = GetNextBoss();
        Debug.Log($"Selected Boss: {(result != null ? result.bossName : "NULL")}");
    }

    // Context menu for testing in editor
    [Button, GUIColor(1, 0, 0)]
    private void ClearSelection()
    {
        history.PreviousBosses.Clear();
    }
}