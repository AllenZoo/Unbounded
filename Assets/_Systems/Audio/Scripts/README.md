# Map Music System

A scalable audio system for managing map music states in Unity. This system supports multiple music "modes" based on gameplay state with smooth transitions between tracks.

## Overview

The system consists of four main components:

1. **MusicState** - Enum defining all possible music modes
2. **MapMusicConfig** - ScriptableObject for storing music tracks per map
3. **MusicManager** - Singleton that manages music playback and transitions
4. **MapMusicInitializer** - Component to register map music on scene load

## Features

- ✅ Multiple music states (Peaceful, BossFight, Combat, LowHealth, Victory, Cutscene)
- ✅ Smooth fade in/out transitions between tracks
- ✅ Only one music track active at a time
- ✅ Maps can define their own music for each state
- ✅ Automatic boss music switching via event system
- ✅ Easy to extend with new music states
- ✅ Proper separation of concerns
- ✅ Integrates with existing AudioManager for volume control

## Setup

### 1. Ensure MusicManager exists in your scene

The `MusicManager` should be placed on a persistent GameObject in your scene (e.g., in the same scene as `AudioManager`).

**Setup steps:**
1. Create an empty GameObject named "MusicManager" (or add the component to an existing audio GameObject)
2. Add the `MusicManager` component
3. The component will automatically create/find an AudioSource for music playback

### 2. Create a MapMusicConfig ScriptableObject

For each map that needs music:

1. Right-click in Project window → Create → Audio → Map Music Config
2. Name it appropriately (e.g., "ForestMapMusic", "DungeonMapMusic")
3. Set the "Map Name" field
4. Add AudioClips for each music state you want:
   - **Peaceful** - Ambient music during exploration
   - **BossFight** - Intense music during boss fights
   - **Combat** - General combat music (optional)
   - **LowHealth** - Tension music when low health (optional)
   - **Victory** - Celebratory music after victory (optional)
   - **Cutscene** - Cutscene music (optional)

### 3. Add MapMusicInitializer to your map scene

1. Create an empty GameObject in your map scene (e.g., "MapMusicSetup")
2. Add the `MapMusicInitializer` component
3. Assign your `MapMusicConfig` ScriptableObject
4. Set the "Initial Music State" (usually `Peaceful`)
5. Leave "Auto Initialize" checked for automatic setup on scene load

## Usage

### Automatic Boss Music Transitions

The system automatically listens to boss fight events:

- When `OnBossFightStartEvent` is fired → switches to **BossFight** music
- When `OnBossFightEndEvent` is fired → switches back to **Peaceful** music

No additional code needed! Just ensure your boss uses `BossFightInitializer` or fires these events.

### Manual Music State Changes

You can manually change music states from any script:

```csharp
// Switch to combat music
MusicManager.Instance.SetMusicState(MusicState.Combat);

// Switch to peaceful music
MusicManager.Instance.SetMusicState(MusicState.Peaceful);

// Stop all music
MusicManager.Instance.SetMusicState(MusicState.None);
```

### Registering Music for a New Map

```csharp
// Register a new map's music configuration
MapMusicConfig config = // ... load your config
MusicManager.Instance.RegisterMapMusic(config, MusicState.Peaceful);
```

### Query Current State

```csharp
// Get current music state
MusicState currentState = MusicManager.Instance.GetCurrentState();

// Get current map config
MapMusicConfig config = MusicManager.Instance.GetCurrentMapConfig();
```

## Architecture

### Event-Driven Design

The system uses the existing EventBus system to respond to gameplay events:

```csharp
// Boss fight starts
EventBus<OnBossFightStartEvent>.Call(new OnBossFightStartEvent { bossName = "Dragon" });
// → MusicManager automatically switches to BossFight music

// Boss fight ends
EventBus<OnBossFightEndEvent>.Call(new OnBossFightEndEvent { bossName = "Dragon" });
// → MusicManager automatically switches back to Peaceful music
```

### Separation of Concerns

- **MapMusicConfig** - Data layer (what music to play)
- **MusicManager** - Logic layer (how to play music, transitions)
- **MapMusicInitializer** - Integration layer (when to load music)
- **Boss systems** - Trigger layer (when to change music)

### Smooth Transitions

All music transitions use coroutines with fade in/out:

1. Fade out current track (transitionDuration / 2)
2. Stop current track
3. Start new track at volume 0
4. Fade in new track (transitionDuration / 2)

## Extending the System

### Adding New Music States

1. Open `MusicState.cs`
2. Add your new state to the enum:

```csharp
public enum MusicState
{
    // ... existing states
    MyNewState,  // Add your new state here
}
```

3. Add the corresponding track to your `MapMusicConfig` ScriptableObjects
4. Trigger the state from your gameplay code:

```csharp
MusicManager.Instance.SetMusicState(MusicState.MyNewState);
```

### Adding Automatic State Transitions

Add event listeners in `MusicManager.Awake()`:

```csharp
// Example: Switch to low health music automatically
var lowHealthBinding = new EventBinding<OnLowHealthEvent>(OnLowHealth);
EventBus<OnLowHealthEvent>.Register(lowHealthBinding);

private void OnLowHealth(OnLowHealthEvent e)
{
    SetMusicState(MusicState.LowHealth);
}
```

### Customizing Transition Duration

Adjust the `transitionDuration` field in the MusicManager inspector (default: 1.5 seconds).

## Example: Complete Map Setup

1. **Create MapMusicConfig**
   - Create: Assets/Audio/Configs/ForestMapMusic.asset
   - Assign:
     - Peaceful: "ForestAmbient.mp3"
     - BossFight: "BossTheme.mp3"

2. **Setup Scene**
   - Add GameObject "MapMusicSetup"
   - Add MapMusicInitializer component
   - Assign ForestMapMusic config
   - Set Initial Music State: Peaceful

3. **Boss Setup**
   - Boss prefab has `BossFightInitializer` component
   - When boss spawns/aggros, it calls `StartBossFight()`
   - Music automatically switches to boss theme

4. **Done!**
   - Peaceful music plays on map load
   - Boss music plays when boss fight starts
   - Peaceful music returns when boss is defeated

## Integration with Existing Systems

### AudioManager Integration

The `MusicManager` uses its own AudioSource for music playback. To integrate with the existing volume controls:

1. Ensure the MusicManager's AudioSource is connected to the background music volume
2. Or add volume control forwarding in MusicManager.Start():

```csharp
// In MusicManager.Start()
if (AudioManager.Instance != null && AudioManager.Instance.backgroundMusicVolume != null)
{
    musicAudioSource.volume = AudioManager.Instance.backgroundMusicVolume.Value / 100f;
    AudioManager.Instance.backgroundMusicVolume.OnValueChanged += (float volume) => {
        musicAudioSource.volume = volume / 100f;
    };
}
```

### BackgroundMusicSetter Compatibility

The new `MusicManager` provides more features than `BackgroundMusicSetter`. Consider:

- Use `MusicManager` for maps with dynamic music (boss fights, combat states)
- Use `BackgroundMusicSetter` for static music scenes (main menu, credits)
- Or migrate all music to `MusicManager` for consistency

## Troubleshooting

**Music doesn't play on scene load**
- Ensure MusicManager GameObject exists in scene
- Check that MapMusicInitializer has a valid MapMusicConfig assigned
- Verify AudioClips are assigned in the MapMusicConfig

**Music doesn't switch during boss fight**
- Verify boss has BossFightInitializer component
- Check that boss fires OnBossFightStartEvent
- Ensure MapMusicConfig has a track assigned for BossFight state

**Transitions are too fast/slow**
- Adjust "Transition Duration" in MusicManager inspector
- Range: 0.1 to 5 seconds (default: 1.5)

**Multiple music tracks playing at once**
- MusicManager uses a single AudioSource - this shouldn't happen
- Check if BackgroundMusicSetter is also active in the scene

## Performance Considerations

- ✅ Single AudioSource for music (minimal overhead)
- ✅ Coroutines for transitions (no per-frame updates when stable)
- ✅ Dictionary lookups for music tracks (O(1) access)
- ✅ Proper event cleanup to prevent memory leaks
- ✅ No allocations during gameplay (only during transitions)

## Future Enhancements

Potential features to add in the future:

- [ ] Music layers system (add/remove layers dynamically)
- [ ] Crossfade with overlap option
- [ ] Music sync points for seamless transitions
- [ ] Volume curves for more advanced transitions
- [ ] Music playlist support for random variations
- [ ] Save/load music state for game saves
- [ ] Audio mixer integration for advanced effects

## API Reference

### MusicManager

**Public Methods:**
- `RegisterMapMusic(MapMusicConfig config, MusicState initialState)` - Register map music
- `SetMusicState(MusicState newState)` - Change music state
- `GetCurrentState()` - Get current music state
- `GetCurrentMapConfig()` - Get current map config

**Inspector Fields:**
- `musicAudioSource` - AudioSource for music playback (auto-assigned)
- `transitionDuration` - Fade in/out duration (default: 1.5s)

### MapMusicConfig

**Public Methods:**
- `GetTrack(MusicState state)` - Get audio clip for state
- `HasTrack(MusicState state)` - Check if track exists for state

**Public Properties:**
- `MapName` - Name of the map

### MapMusicInitializer

**Public Methods:**
- `InitializeMapMusic()` - Manually initialize map music

**Inspector Fields:**
- `mapMusicConfig` - The map's music configuration
- `initialMusicState` - Starting music state (default: Peaceful)
- `autoInitialize` - Auto-register on scene load (default: true)

### MusicState Enum

Values:
- `None` - No music
- `Peaceful` - Exploration/ambient music
- `BossFight` - Boss battle music
- `Combat` - General combat music
- `LowHealth` - Low health tension music
- `Victory` - Victory celebration music
- `Cutscene` - Cutscene music
