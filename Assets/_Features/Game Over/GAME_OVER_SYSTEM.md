# Game Over Score Summary System (UI Toolkit)

## Overview
This system provides a scalable, decoupled architecture for displaying game over score summaries to the player using **Unity's UI Toolkit**. It follows Unity best practices by separating concerns between gameplay logic, score calculation, and UI presentation.

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

#### 3. **GameOverController** (`Assets/UI Toolkit/Controllers/GameOverController.cs`)
A MonoBehaviour controller that manages the UI Toolkit-based Game Over display.

**Responsibilities:**
- Listens for `OnGameOverEvent`
- Updates `GameOverUIData` with formatted score information
- Controls UI visibility through `GameOverContext`
- Handles button clicks (Retry, Main Menu)
- Does NOT calculate scores (only displays provided data)

**Key Features:**
- UI Toolkit integration with data binding
- Automatic event registration/cleanup
- VisualElement-based button handling
- Context-driven visibility management

#### 4. **GameOverContext** (`Assets/UI Toolkit/Data/GameOverContext.cs`)
A ScriptableObject that manages the state of the Game Over UI.

**Responsibilities:**
- Tracks whether Game Over UI is open/closed
- Stores current score summary
- Notifies listeners when state changes

#### 5. **GameOverUIData** (`Assets/UI Toolkit/Data/GameOverUIData.cs`)
A ScriptableObject that holds formatted data for UI Toolkit data binding.

**Responsibilities:**
- Converts `ScoreSummaryData` to display strings
- Provides properties for UI Toolkit data binding
- Formats time values (MM:SS format)

#### 6. **GameOverUI.uxml** (`Assets/UI Toolkit/GameOverUI.uxml`)
The UI Toolkit visual tree that defines the Game Over screen layout.

**Features:**
- Responsive layout using VisualElements
- Data binding to `GameOverUIData` properties
- Styled with USS for consistent appearance
- Contains retry and main menu buttons

#### 7. **GameManagerComponent Updates** (`Assets/Scripts/Systems/GameManagement/GameManagerComponent.cs`)
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
GameOverController.OnGameOver() → Receives event
    ↓
GameOverUIData.UpdateFromScoreSummary() → Formats data for display
    ↓
GameOverContext.Open() → Signals UI to show
    ↓
UI Document displays Game Over screen via data binding
```
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

See `GAME_OVER_SETUP_GUIDE_UITOOLKIT.md` for detailed step-by-step instructions.

**Quick Setup:**

1. **Create UI Document GameObject**
   - Add a UI Toolkit > UI Document to your scene
   - Assign `GameOverUI.uxml` as the source asset
   - Configure Panel Settings

2. **Create ScriptableObject Assets**
   - Create a `GameOverContext` instance
   - Create a `GameOverUIData` instance

3. **Add GameOverController Component**
   - Add the `GameOverController` script to the UI Document GameObject
   - Assign the UI Document, Context, and Data references

### Triggering Game Over Programmatically

```csharp
// The system triggers automatically on player death
// But you can also trigger it manually:
ScoreSummaryData summary = ScoreSummaryData.FromRunData(RunTracker.Instance.CurrentRun);
EventBus<OnGameOverEvent>.Raise(new OnGameOverEvent { scoreSummary = summary });
```

### Extending the System

#### Adding a High Score Comparison
### Extending the System

#### Adding a High Score Comparison

```csharp
// In GameOverUIData.cs - add a new property
public string highScoreText;

// In GameOverUIData.UpdateFromScoreSummary()
int highScore = PlayerPrefs.GetInt("HighScore", 0);
if (data.totalScore > highScore)
{
    PlayerPrefs.SetInt("HighScore", data.totalScore);
    highScoreText = "NEW HIGH SCORE!";
}
else
{
    highScoreText = $"High Score: {highScore:N0}";
}

// In GameOverUI.uxml - add a label with data binding
<ui:Label text="High Score: 0" name="HighScoreLabel">
    <Bindings>
        <ui:DataBinding property="text" data-source-path="highScoreText" binding-mode="ToTarget"/>
    </Bindings>
</ui:Label>
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

// In GameOverUIData - add properties and update method
public string rewardsText;

// Update in UpdateFromScoreSummary()
if (data.unlockedItems != null && data.unlockedItems.Count > 0)
{
    rewardsText = $"Unlocked: {string.Join(", ", data.unlockedItems)}";
}
```

## Dependencies

### Existing Systems Used
- **ScoreCalculator**: For score calculation (reused, not duplicated)
- **RunTracker**: For run data collection
- **EventBus**: For event-driven communication
- **UI Toolkit**: For modern, performant UI rendering
- **UIDocument**: For UI Toolkit integration

### New Dependencies
- **UI Toolkit Package**: Unity's modern UI solution (already in project)

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
- GameOverController logs when displaying summary
- Use GameOverUITester (F9/F10) for manual testing

## Notes

- The `MainMenuButton` functionality is marked as TODO and requires scene loading implementation
- The system automatically hides the Game Over UI on startup
- UI Toolkit provides better performance than Canvas UI
- Data binding automatically updates UI when data changes
- USS styling allows easy visual customization without code changes
