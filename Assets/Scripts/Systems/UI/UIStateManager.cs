using System;
using UnityEngine;

/// <summary>
/// Singleton manager that tracks the global UI state.
/// Used to determine if any UI elements (modals, inventories, pages) are currently active.
/// </summary>
public class UIStateManager : Singleton<UIStateManager>
{
    /// <summary>
    /// Event fired when the UI active state changes.
    /// </summary>
    public static event Action<bool> OnUIActiveStateChanged;

    private int activeUICount = 0;

    /// <summary>
    /// Returns true if any UI element is currently active.
    /// </summary>
    public bool IsUIActive => activeUICount > 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Ensures a UIStateManager instance exists. Creates one if needed.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void EnsureInstance()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("UIStateManager");
            go.AddComponent<UIStateManager>();
            DontDestroyOnLoad(go);
        }
    }

    /// <summary>
    /// Register that a UI element has been opened.
    /// </summary>
    public void RegisterUIOpen()
    {
        activeUICount++;
        if (activeUICount == 1)
        {
            OnUIActiveStateChanged?.Invoke(true);
        }
    }

    /// <summary>
    /// Register that a UI element has been closed.
    /// </summary>
    public void RegisterUIClose()
    {
        activeUICount = Mathf.Max(0, activeUICount - 1);
        if (activeUICount == 0)
        {
            OnUIActiveStateChanged?.Invoke(false);
        }
    }

    /// <summary>
    /// Reset the UI state count. Useful for cleanup or testing.
    /// </summary>
    public void ResetUIState()
    {
        activeUICount = 0;
        OnUIActiveStateChanged?.Invoke(false);
    }
}
