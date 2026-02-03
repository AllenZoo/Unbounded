using UnityEngine;

/// <summary>
/// Test script to verify that player attack input is properly disabled when UI is active.
/// Attach this to a GameObject in the scene and press the test keys to verify functionality.
/// </summary>
public class UIStateTest : MonoBehaviour
{
    private void Update()
    {
        // Press 'T' to test UI state reporting
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestUIState();
        }
        
        // Press 'U' to simulate opening UI
        if (Input.GetKeyDown(KeyCode.U))
        {
            SimulateUIOpen();
        }
        
        // Press 'I' to simulate closing UI
        if (Input.GetKeyDown(KeyCode.I))
        {
            SimulateUIClose();
        }
    }
    
    private void TestUIState()
    {
        if (UIStateManager.Instance != null)
        {
            Debug.Log($"[UIStateTest] UI State: {(UIStateManager.Instance.IsUIActive ? "ACTIVE (Input Blocked)" : "INACTIVE (Input Allowed)")}");
        }
        else
        {
            Debug.LogError("[UIStateTest] UIStateManager instance is null!");
        }
    }
    
    private void SimulateUIOpen()
    {
        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.RegisterUIOpen();
            Debug.Log("[UIStateTest] Simulated UI Open - Attack input should now be disabled");
            TestUIState();
        }
    }
    
    private void SimulateUIClose()
    {
        if (UIStateManager.Instance != null)
        {
            UIStateManager.Instance.RegisterUIClose();
            Debug.Log("[UIStateTest] Simulated UI Close - Attack input should now be enabled");
            TestUIState();
        }
    }
}
