using System;
using UnityEngine;

/// <summary>
/// Context ScriptableObject that holds state for the Game Over UI.
/// This acts as a bridge between the game state and the UI Toolkit display.
/// </summary>
[CreateAssetMenu(fileName = "GameOverContext", menuName = "System/Contexts/Game Over Context", order = 1)]
public class GameOverContext : ScriptableObject
{
    public bool IsOpen { get; private set; }
    public ScoreSummaryData CurrentScoreSummary { get; private set; }
    
    public Action OnChanged;

    /// <summary>
    /// Opens the Game Over UI with the provided score summary data.
    /// </summary>
    public void Open(ScoreSummaryData scoreSummary)
    {
        IsOpen = true;
        CurrentScoreSummary = scoreSummary;
        OnChanged?.Invoke();
        Debug.Log($"GameOverContext: Opened with score {scoreSummary?.totalScore}");
    }

    /// <summary>
    /// Closes the Game Over UI.
    /// </summary>
    public void Close()
    {
        IsOpen = false;
        CurrentScoreSummary = null;
        OnChanged?.Invoke();
        Debug.Log("GameOverContext: Closed");
    }

    /// <summary>
    /// Resets the context (useful for cleanup).
    /// </summary>
    private void OnDisable()
    {
        IsOpen = false;
        CurrentScoreSummary = null;
    }
}
