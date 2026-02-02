using UnityEngine;

/// <summary>
/// Component attached to boss prefabs or placed in boss scenes.
/// Triggers the boss fight start event when the boss is initialized.
/// </summary>
public class BossFightInitializer : MonoBehaviour
{
    [SerializeField] private string bossName = "Boss";
    [SerializeField] private bool autoStartInStart = true;

    private bool hasStarted = false;

    private void Start()
    {
        if (autoStartInStart)
        {
            StartBossFight();
        }
    }

    /// <summary>
    /// Manually triggers the boss fight start event.
    /// </summary>
    public void StartBossFight()
    {
        if (hasStarted)
        {
            Debug.LogWarning($"Boss fight already started for {bossName}");
            return;
        }

        // Use gameObject name if bossName is not set
        string actualBossName = string.IsNullOrEmpty(bossName) ? gameObject.name : bossName;

        // Fire the boss fight start event
        EventBus<OnBossFightStartEvent>.Call(new OnBossFightStartEvent { bossName = actualBossName });
        
        hasStarted = true;
        Debug.Log($"Boss fight started: {actualBossName}");
    }
}
