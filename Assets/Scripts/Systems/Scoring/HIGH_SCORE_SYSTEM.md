# High Score & Run History System

## Overview

The High Score & Run History System tracks player performance across multiple game runs, persists this data between sessions, and provides a UI for viewing historical run data.

## Components

### Data Structures

#### RunHistoryData
Location: `Assets/Scripts/Systems/Scoring/RunHistoryData.cs`

Stores individual run records including:
- Final score
- Bosses defeated
- Total damage dealt
- Run duration
- Timestamp
- List of weapons used

#### WeaponUsageData
Location: `Assets/Scripts/Systems/Scoring/RunHistoryData.cs`

Tracks weapon usage during a run:
- Weapon ID (GUID)
- Weapon name
- Equip time

#### GameData Updates
Location: `Assets/Scripts/Systems/DataPersistence/GameData.cs`

Added fields:
- `highScore` - Tracks the highest score achieved
- `runHistory` - List of past run records (max 50 runs)

### Manager Component

#### RunHistoryManager
Location: `Assets/Scripts/Systems/Scoring/RunHistoryManager.cs`

Singleton component that:
- Listens to `OnGameOverEvent` to capture completed runs
- Tracks weapons used during gameplay via inventory changes
- Implements `IDataPersistence` to save/load data
- Maintains circular buffer of run history (max 50 runs)
- Automatically updates high score

**Key Methods:**
- `StartNewRun()` - Initialize tracking for a new run
- `LoadData(GameData)` - Load high score and history from save
- `SaveData(GameData)` - Save high score and history

**Integration Points:**
- Hooked into `GameManagerComponent.StartNewRun()`
- Automatically registered with `DataPersistenceHandler`

### UI Components

#### HighScoreUIData
Location: `Assets/UI Toolkit/Data/HighScoreOverlay/HighScoreUIData.cs`

ScriptableObject that formats run history for UI display:
- Converts raw data to formatted strings
- Manages list of displayable run entries
- Supports UI Toolkit data binding

#### HighScoreContext
Location: `Assets/UI Toolkit/Data/HighScoreOverlay/HighScoreContext.cs`

ScriptableObject that manages UI state:
- Controls open/close state
- Fires `OnChanged` event when state changes
- Acts as bridge between game logic and UI

#### HighScoreController
Location: `Assets/UI Toolkit/Controllers/HighScoreController.cs`

MonoBehaviour that:
- Manages UI Toolkit document
- Responds to context changes
- Dynamically builds run history list
- Handles button interactions (Back)

#### HighScoreOpener
Location: `Assets/Scripts/Systems/UI/HighScoreOpener.cs`

Simple helper component to open high score screen from buttons/UI elements.

### UI Assets

#### HighScoreUI.uxml
Location: `Assets/UI Toolkit/HighScoreUI.uxml`

UI Toolkit UXML document defining the visual layout:
- High score display
- Scrollable run history list
- Back button
- Uses data binding for dynamic content

#### styles.uss
Location: `Assets/UI Toolkit/styles.uss`

Added styles for run entry display:
- `.run-entry` - Container for each run
- `.run-entry-header` - Score and timestamp row
- `.run-score` - Gold-colored score text
- `.run-timestamp` - Gray timestamp text
- `.run-stats` - Statistics row
- `.run-weapons` - Weapon list display

## Setup Instructions

### 1. Add RunHistoryManager to Scene

Add the `RunHistoryManager` component to a persistent GameObject in your game:
1. Create or select a GameObject in the "Persistent" scene
2. Add the `RunHistoryManager` component
3. The component will auto-register with the data persistence system

### 2. Create ScriptableObject Assets

Create the required ScriptableObject assets:

1. **HighScoreContext**:
   - Right-click in Project → Create → System → Contexts → High Score Context
   - Name it "HighScoreContext"

2. **HighScoreUIData**:
   - Right-click in Project → Create → System → UI Toolkit → High Score Data
   - Name it "HighScoreUIData"

### 3. Setup High Score UI Scene/Overlay

1. Create a GameObject with a `UIDocument` component
2. Assign the `HighScoreUI.uxml` to the UIDocument
3. Add the `HighScoreController` component to the same GameObject
4. In the HighScoreController inspector:
   - Assign the `UIDocument` reference
   - Assign the `HighScoreContext` asset
   - Assign the `HighScoreUIData` asset
   - Assign the main menu scene reference

### 4. Add High Score Button to Main Menu

To add a button that opens the high score screen:

1. Add a button to your main menu
2. Add the `HighScoreOpener` component to a GameObject
3. Assign the `HighScoreContext` asset to the HighScoreOpener
4. Wire the button's onClick event to call `HighScoreOpener.OpenHighScoreScreen()`

Alternatively, you can directly call `HighScoreContext.Open()` from any code.

## How It Works

### Run Tracking Flow

1. **Run Start**: `GameManagerComponent.StartNewRun()` calls `RunHistoryManager.StartNewRun()`
2. **During Run**: Inventory changes trigger weapon tracking
3. **Run End**: `OnGameOverEvent` triggers `RunHistoryManager.HandleGameOver()`
4. **Data Save**: Run data is added to history, high score is updated, data is persisted

### Data Persistence

The system integrates with the existing `DataPersistenceHandler`:
- Data is saved to `GameData` via `IDataPersistence` interface
- Automatic save on game over
- Loads automatically when game starts
- Uses existing JSON serialization system

### Circular Buffer

Run history maintains a maximum of 50 runs:
- New runs are added to the end of the list
- When limit is exceeded, oldest run is removed
- Most recent runs are displayed first in UI

### Weapon Tracking

Weapons are tracked via inventory modifications:
- Each time inventory changes, the equipped weapon is checked
- Unique weapons are recorded once per run (by weapon ID)
- Weapon names are stored for display

## Testing

### Manual Testing

1. **First Run**:
   - Start a new game
   - Equip different weapons during gameplay
   - Complete or fail the run
   - Check that score is saved

2. **High Score Update**:
   - Complete another run with a higher score
   - Verify high score updates correctly

3. **Run History**:
   - Open the high score screen
   - Verify all completed runs are listed
   - Check that weapons are displayed correctly

4. **Persistence**:
   - Close and reopen the game
   - Verify high score and run history persist

5. **Circular Buffer**:
   - Complete more than 50 runs (if feasible)
   - Verify oldest runs are removed

### Debug Logging

The system logs key events:
- `[RunHistoryManager] Started tracking new run`
- `[RunHistoryManager] Tracked weapon: {weaponName}`
- `[RunHistoryManager] New high score: {score}`
- `[RunHistoryManager] Saved run: Score={score}, Weapons={count}`
- `[RunHistoryManager] Loaded: High Score={score}, History size={count}`

## Future Enhancements

Potential improvements:
- Add filters/sorting to run history (by date, score, etc.)
- Add detailed per-run breakdown view
- Add statistics (average score, favorite weapons, etc.)
- Add achievements based on run history
- Export run history to file
- Compare runs side-by-side
