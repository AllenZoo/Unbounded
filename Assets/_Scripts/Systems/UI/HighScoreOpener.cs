using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Simple component to open the high score screen.
/// Attach to a button or UI element to trigger the high score view.
/// </summary>
public class HighScoreOpener : MonoBehaviour
{
    [Required, SerializeField] private HighScoreContext highScoreContext;

    /// <summary>
    /// Opens the high score screen. Call this from a button click or other UI event.
    /// </summary>
    public void OpenHighScoreScreen()
    {
        if (highScoreContext == null)
        {
            Debug.LogError("HighScoreOpener: HighScoreContext is not assigned!");
            return;
        }

        Debug.Log("HighScoreOpener: Opening high score screen");
        highScoreContext.Open();
    }
}
