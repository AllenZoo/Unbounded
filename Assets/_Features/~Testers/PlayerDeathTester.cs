using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Script for testing player death by triggering it with a hotkey.
/// Attach this to a GameObject in your scene and press the specified key to test.
/// 
/// This is for TESTING ONLY - remove from production builds.
/// </summary>
public class PlayerDeathTester : MonoBehaviour
{
    [Header("Test Controls")]
    [Tooltip("Key to trigger player death")]
    public KeyCode killPlayerKey = KeyCode.F11;

    private void Update()
    {
        // Trigger death when the specified key is pressed
        if (Input.GetKeyDown(killPlayerKey))
        {
            KillPlayer();
        }
    }

    /// <summary>
    /// Triggers player death by calling OnDeathEvent on the player's LocalEventHandler.
    /// This simulates a natural death and allows PlayerDeathTracker to handle the global event.
    /// </summary>
    [Button("Kill Player")]
    public void KillPlayer()
    {
        if (PlayerSingleton.Instance == null)
        {
            Debug.LogWarning("[PlayerDeathTester] Cannot kill player: PlayerSingleton.Instance is null. Make sure the player is in the scene.");
            return;
        }

        // The player usually has a LocalEventHandler component. 
        // We try to find it on the player object or its children.
        LocalEventHandler leh = PlayerSingleton.Instance.GetComponentInChildren<LocalEventHandler>();
        
        if (leh != null)
        {
            Debug.Log($"[PlayerDeathTester] Triggering player death via LocalEventHandler (Press {killPlayerKey})");
            leh.Call(new OnDeathEvent());
        }
        else
        {
            // Fallback: If for some reason we can't find the local handler, 
            // we trigger the global event directly to ensure the game over sequence starts.
            Debug.LogWarning("[PlayerDeathTester] Could not find LocalEventHandler on player. Triggering global OnPlayerDeathEvent instead.");
            EventBus<OnPlayerDeathEvent>.Call(new OnPlayerDeathEvent());
        }
    }

    private void OnGUI()
    {
        // Display test instructions in the game view
        // Positioned slightly lower to avoid overlapping with other tester UIs if they exist
        GUILayout.BeginArea(new Rect(10, 150, 300, 80));
        GUILayout.Box("Player Death Tester");
        GUILayout.Label($"Press {killPlayerKey} - Kill Player");
        GUILayout.Label("Or use Inspector button");
        GUILayout.EndArea();
    }
}