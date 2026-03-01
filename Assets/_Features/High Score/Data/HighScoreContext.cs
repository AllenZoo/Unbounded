using System;
using UnityEngine;

/// <summary>
/// Context ScriptableObject that holds state for the High Score UI.
/// This acts as a bridge between the game state and the UI Toolkit display.
/// </summary>
[CreateAssetMenu(fileName = "HighScoreContext", menuName = "System/Contexts/High Score Context", order = 2)]
public class HighScoreContext : ScriptableObject
{
    public bool IsOpen { get; private set; }
    
    public Action OnChanged;

    /// <summary>
    /// Opens the High Score UI.
    /// </summary>
    public void Open()
    {
        IsOpen = true;
        OnChanged?.Invoke();
        Debug.Log("HighScoreContext: Opened");
    }

    /// <summary>
    /// Closes the High Score UI.
    /// </summary>
    public void Close()
    {
        IsOpen = false;
        OnChanged?.Invoke();
        Debug.Log("HighScoreContext: Closed");
    }

    /// <summary>
    /// Resets the context (useful for cleanup).
    /// </summary>
    private void OnDisable()
    {
        IsOpen = false;
    }
}
