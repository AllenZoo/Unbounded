# Game Over Score Summary System

## Overview
This system provides a scalable, decoupled architecture for displaying game over score summaries to the player. It follows Unity best practices by separating concerns between gameplay logic, score calculation, and UI presentation.

## Architecture

### Components

#### 1. **ScoreSummaryData** (`Assets/Scripts/Systems/Scoring/ScoreSummaryData.cs`)
A data transfer object that encapsulates all score and statistics information for display.

**Fields:**
- `totalScore`: Final calculated score
- `damageScore`: Score component from damage dealt
- `timeScore`: Score component from fight duration
- `bossesDefeated`: Number of bosses defeated in the run
- `totalDamageDealt`: Total damage dealt across all fights
- `totalTimeSurvived`: Total time spent fighting bosses

**Key Method:**
```csharp
ScoreSummaryData.FromRunData(RunData runData)
```
Creates a summary by delegating to the existing `ScoreCalculator`. This ensures score calculation logic remains centralized.

#### 2. **OnGameOverEvent** (`Assets/Scripts/Event/IEvent.cs`)
A global event that signals the end of a game run and carries the score summary data.

```csharp
public struct OnGameOverEvent : IGlobalEvent
{
    public ScoreSummaryData scoreSummary;
}
```

#### 3. **GameOverUI** (`Assets/Scripts/Systems/UI/Page/GameOverUI.cs`)
A UI component that displays the score summary. Extends `PageUI` to integrate with the existing UI overlay system.

**Responsibilities:**
- Listens for `OnGameOverEvent`
- Displays score breakdown and statistics
- Provides optional retry/main menu buttons
- Does NOT calculate scores (only displays provided data)

**Key Features:**
- Automatic event registration/cleanup
- Null-safe UI updates
- Time formatting helper (MM:SS format)
- Extensible for future features (high scores, rewards, etc.)

#### 4. **GameManagerComponent Updates** (`Assets/Scripts/Systems/GameManagement/GameManagerComponent.cs`)
The game manager now triggers the game over flow when the player dies:

```csharp
public void OnPlayerDeath()
{
    // End run and calculate score
    int finalScore = RunTracker.Instance.EndRun();
    
    // Create summary and trigger event
    ScoreSummaryData summary = ScoreSummaryData.FromRunData(RunTracker.Instance.CurrentRun);
    EventBus<OnGameOverEvent>.Raise(new OnGameOverEvent { scoreSummary = summary });
    
    ChangeState(GameState.RunEnd);
}
```

## Flow Diagram

```
Player Dies
    ↓
GameManagerComponent.OnPlayerDeath()
    ↓
RunTracker.EndRun() → Calculates score using ScoreCalculator
    ↓
ScoreSummaryData.FromRunData() → Packages data for UI
    ↓
EventBus<OnGameOverEvent>.Raise() → Broadcasts event
    ↓
GameOverUI.OnGameOver() → Receives event and displays summary
```

## Design Principles

### 1. **Separation of Concerns**
- **Scoring Logic**: `ScoreCalculator` (unchanged, reused)
- **Run Tracking**: `RunTracker` (unchanged, reused)
- **Data Transfer**: `ScoreSummaryData`
- **Event Broadcasting**: `OnGameOverEvent`
- **UI Display**: `GameOverUI`

### 2. **Loose Coupling**
- GameManager doesn't know about UI components
- GameOverUI doesn't know about score calculation
- Communication through events (EventBus pattern)

### 3. **Extensibility**
The system is designed to easily support future features:
- **High Scores**: Add comparison logic in `GameOverUI` or create a separate `HighScoreManager`
- **Rewards/Unlocks**: Extend `ScoreSummaryData` with reward information
- **Additional Statistics**: Add fields to `ScoreSummaryData` (e.g., `enemiesKilled`, `itemsCollected`)
- **Animations**: Override `DisplayScoreSummary()` in a subclass
- **Localization**: Replace hard-coded strings with localization keys

### 4. **Testability**
- `ScoreSummaryData.FromRunData()` can be unit tested independently
- `GameOverUI` can be tested by raising events manually
- No tight dependencies on game state

## Usage

### Setting Up in Unity Editor

1. **Create a Canvas** for the Game Over UI (if not already present)

2. **Add GameOverUI Component**
   - Create a new GameObject as a child of the Canvas
   - Add the `GameOverUI` component
   - Configure the Canvas and Collider2D (required by PageUI)

3. **Assign UI Elements**
   - Create TextMeshProUGUI elements for:
     - Total Score
     - Damage Score
     - Time Score
     - Bosses Defeated
     - Total Damage
     - Time Survived
   - Optionally add Retry and Main Menu buttons
   - Assign references in the GameOverUI inspector

4. **Assign PageUIContext**
   - Use the existing `GameOverPage.asset` or create a new PageUIContext
   - Assign it to the GameOverUI's pageUIContext field

### Triggering Game Over Programmatically

```csharp
// The system triggers automatically on player death
// But you can also trigger it manually:
ScoreSummaryData summary = ScoreSummaryData.FromRunData(RunTracker.Instance.CurrentRun);
EventBus<OnGameOverEvent>.Raise(new OnGameOverEvent { scoreSummary = summary });
```

### Extending the System

#### Adding a High Score Comparison

```csharp
// In GameOverUI.cs
[SerializeField] private TextMeshProUGUI highScoreText;

public void DisplayScoreSummary(ScoreSummaryData data)
{
    // ... existing code ...
    
    int highScore = PlayerPrefs.GetInt("HighScore", 0);
    if (data.totalScore > highScore)
    {
        PlayerPrefs.SetInt("HighScore", data.totalScore);
        highScoreText.text = "NEW HIGH SCORE!";
    }
    else
    {
        highScoreText.text = $"High Score: {highScore:N0}";
    }
}
```

#### Adding Reward Information

```csharp
// Extend ScoreSummaryData
public class ScoreSummaryData
{
    // ... existing fields ...
    public List<string> unlockedItems;
    public int coinsEarned;
}

// Update GameOverUI to display rewards
if (data.unlockedItems != null && data.unlockedItems.Count > 0)
{
    rewardsText.text = $"Unlocked: {string.Join(", ", data.unlockedItems)}";
}
```

## Dependencies

### Existing Systems Used
- **ScoreCalculator**: For score calculation (reused, not duplicated)
- **RunTracker**: For run data collection
- **EventBus**: For event-driven communication
- **PageUI**: For UI overlay management
- **UIOverlayManager**: For page layering and visibility

### New Dependencies
- **TextMeshPro**: For text rendering (standard in Unity)

## Future Enhancements

Potential improvements to consider:
1. **Score Animations**: Animate numbers counting up
2. **Star Rating**: Display 1-5 stars based on performance
3. **Detailed Boss Breakdown**: Show per-boss statistics
4. **Share Functionality**: Screenshot and share results
5. **Leaderboard Integration**: Submit scores to online leaderboard
6. **Replay System**: Save and replay the run

## Testing

### Manual Testing Checklist
- [ ] Game over screen appears when player dies
- [ ] All score values display correctly
- [ ] Score breakdown matches debug logs
- [ ] Retry button restarts the run
- [ ] Time is formatted correctly (MM:SS)
- [ ] UI scales properly on different resolutions

### Debug Commands
The system logs detailed information for debugging:
- RunTracker logs score breakdown when ending a run
- GameManagerComponent logs final score
- GameOverUI logs when displaying summary

## Notes

- The `MainMenuButton` functionality is marked as TODO and requires scene loading implementation
- The system automatically hides the Game Over UI on startup
- All event bindings are properly cleaned up in `OnDestroy()`
- The UI uses the existing PageUI system for consistent overlay management
