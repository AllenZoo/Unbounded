# Run-Based Scoring System

## Overview
This system tracks player performance during a game run and calculates a final score based on damage dealt and time spent fighting bosses.

## Components

### Core Data Classes
- **BossFightData**: Stores data for a single boss fight (name, damage dealt, duration)
- **RunData**: Manages data for an entire run (up to 9 boss fights, total damage)
- **ScoreCalculator**: Calculates final score using configurable formulas

### Integration Components
- **RunTracker** (Singleton): Central tracking system that listens to game events
- **BossDamageTracker**: Attach to boss GameObjects to track damage taken
- **BossFightInitializer**: Triggers boss fight start events (attach to boss prefabs or scenes)

## Usage

### Setup
1. **Add RunTracker to Scene**: Add a GameObject with the `RunTracker` component to your persistent game scene
2. **Attach to Bosses**: Add both `BossDamageTracker` and `BossFightInitializer` components to boss prefabs
3. **Integration**: The system automatically hooks into `GameManagerComponent.StartNewRun()` and `OnPlayerDeath()`

### Example Usage
See `RunTrackerExample.cs` for a complete working example that demonstrates:
- Starting a new run
- Simulating boss fights with damage and timing
- Getting current score and breakdowns
- Ending a run

To test in the editor:
1. Add a GameObject with `RunTrackerExample` component
2. Check "Run Example On Start"
3. Enter play mode to see the system in action

### Events
The system uses these global events:
- `OnBossFightStartEvent`: Fired when a boss fight begins
- `OnBossFightEndEvent`: Fired when a boss is defeated

### Scoring Formula
Current formula (easily extensible):
- **Damage Score**: Total damage × 1.0
- **Time Bonus**: Base 1000 points per boss, minus 0.5 points per second over optimal time (60s)
- Faster kills = higher scores

### Accessing Score Data
```csharp
// Get current score during run
int currentScore = RunTracker.Instance.GetCurrentScore();

// Get final score at run end
int finalScore = RunTracker.Instance.EndRun();

// Get detailed breakdown
ScoreBreakdown breakdown = ScoreCalculator.GetScoreBreakdown(RunTracker.Instance.CurrentRun);
```

## Performance
- No per-frame allocations
- Event-driven updates only
- Minimal memory footprint
- Suitable for production use

## Extensibility
To add new scoring factors:
1. Add fields to `BossFightData` or `RunData`
2. Update `ScoreCalculator.CalculateScore()` with new component
3. Update `ScoreBreakdown` struct if needed for display
