# Unbounded - Copilot Coding Agent Instructions

## Project Overview

Unbounded is a 2D roguelike action game built with Unity. The game features procedurally generated maps, boss encounters, a comprehensive upgrade system, and dynamic difficulty scaling. Players progress through rounds, fighting increasingly difficult bosses with stat scaling mechanics.

## Technology Stack

- **Engine**: Unity 6000.3.6f1 (6000.3 series)
- **Language**: C# (.NET)
- **Rendering**: Universal Render Pipeline (URP) 17.3.0
- **2D Framework**: Unity 2D Feature Set 2.0.2
- **Testing**: Unity Test Framework 1.6.0
- **UI**: UI Toolkit and Unity UI (uGUI) 2.0.0
- **Additional Packages**: Cinemachine 2.10.5, AI Navigation 2.0.9, Timeline 1.8.10

## Project Type and Size

- **Type**: Unity game project (2D roguelike)
- **Codebase Size**: ~350 C# scripts
- **Primary Language**: C# 100%
- **Build Target**: Standalone (Windows primary), WebGL supported

## Build Instructions

### Prerequisites
- Unity Hub installed with Unity Editor version 6000.3.6f1
- No additional build tools or dependencies required outside of Unity

### Opening the Project
1. Open Unity Hub
2. Add the project from `/home/runner/work/Unbounded/Unbounded`
3. Unity will automatically import packages and compile scripts (first import takes 5-10 minutes)

### Building the Game
1. Open the project in Unity Editor
2. Go to File → Build Settings
3. Select target platform (PC, Mac & Linux Standalone or WebGL)
4. Click "Build" or "Build and Run"
5. **Note**: There are no command-line build scripts available. All builds must be done through Unity Editor.

### Running Tests
- Unity Test Framework is available in the project
- Run tests through Unity Editor: Window → General → Test Runner
- No command-line test runner is configured

### Important Notes
- **Always** allow Unity to finish script compilation before making code changes
- Unity uses asset serialization format: Mixed (text for .meta files, binary for scenes)
- No automated CI/CD pipelines are configured in this repository
- Build files are shared separately via Google Drive (see README.md)

## Project Layout and Architecture

### Directory Structure

```
Assets/
├── Scripts/                    # All C# game code
│   ├── Components/            # Game components (entities, attacks, items)
│   │   ├── Player/           # Player-specific components
│   │   ├── Enemies/          # Enemy and boss components  
│   │   ├── Attacks/          # Attack system (RingAttack, ClusterAttack, etc.)
│   │   └── Interactables/    # Interactive objects
│   ├── Systems/              # Core game systems
│   │   ├── GameManagment/    # Game loop, boss selection, tutorial
│   │   ├── Stat/             # Stat system (StatComponent, StatMediator)
│   │   ├── MapGen/           # Procedural map generation
│   │   ├── Inventory/        # Inventory and item management
│   │   ├── Upgrade/          # Upgrade and card system
│   │   ├── Objective/        # Quest/objective system
│   │   ├── Scoring/          # Score tracking
│   │   └── UI/               # UI controllers
│   ├── Scriptable Objs/      # ScriptableObject definitions
│   ├── Utils/                # Utility classes and helpers
│   └── Enums/                # Enum definitions
├── Scenes/                    # Unity scene files
├── Resources/                 # Runtime-loaded assets
├── Prefabs/                   # Prefab objects (not visible in listing)
└── Graphics/                  # Sprites, materials, shaders

ProjectSettings/              # Unity project configuration
Packages/                     # Unity Package Manager dependencies
```

### Key Files and Systems

#### Game Management
- **`Assets/Scripts/Systems/GameManagment/GameManagerComponent.cs`**: Main game loop controller, handles round progression, boss room states
- **`Assets/Scripts/Systems/GameManagment/BossScalingSystem.cs`**: Boss stat scaling based on round number (see SCALING_SYSTEM.md)
- **`Assets/Scripts/Systems/GameManagment/TutorialManager.cs`**: Tutorial system

#### Combat System
- **`Assets/Scripts/Components/Attacks/`**: Attack implementations (RingAttack, ClusterAttack, ProjectileAttack, etc.)
- **`Assets/Scripts/Components/Damageable.cs`**: Health and damage handling
- **Attack Context Fix**: RingAttack and ClusterAttack properly propagate AtkStat and PercentageDamageIncrease (documented in SCALING_SYSTEM.md)

#### Stat System
- **`Assets/Scripts/Systems/Stat/StatComponent.cs`**: Manages entity stats (HP, ATK, DEF, SPD, DEX)
- **`Assets/Scripts/Systems/Stat/StatMediator.cs`**: Applies stat modifiers from equipment, buffs, scaling
- Stats are modified through the mediator pattern to ensure proper application order

#### Map Generation
- **`Assets/Scripts/Systems/MapGen/MapManager.cs`**: Procedural map generation
- **`Assets/Scripts/Systems/MapGen/RoomEventManager.cs`**: Room event handling

#### Other Key Systems
- **Inventory**: `Assets/Scripts/Systems/Inventory/`
- **Upgrade System**: `Assets/Scripts/Systems/Upgrade/`
- **UI Controllers**: `Assets/Scripts/Systems/UI/`

### Configuration Files
- **Project Settings**: `ProjectSettings/` directory contains Unity configuration
- **Package Dependencies**: `Packages/manifest.json` lists all Unity packages
- **No external linting/formatting configs**: Unity C# uses standard .NET conventions

## Development Workflow

### Making Code Changes
1. **Always check compilation**: After opening the project, wait for "Compiling..." in Unity status bar to complete
2. **Script location**: All game code goes in `Assets/Scripts/`
3. **Naming conventions**: PascalCase for classes, methods, and public fields; camelCase for private fields
4. **Component pattern**: Most gameplay features are MonoBehaviour components attached to GameObjects
5. **Event-driven**: Systems communicate via C# events and delegates (see `Assets/Scripts/Event/`)

### Testing Changes
1. **Manual testing in Play Mode**: Primary validation method
   - Open a scene (e.g., main menu or game scene in `Assets/Scenes/`)
   - Click Play button in Unity Editor
   - Test functionality manually
2. **Check console for errors**: Always review Unity Console for runtime errors
3. **No automated integration tests**: Manual play-testing is required

### Common Pitfalls and Workarounds
- **Script compilation errors block Play Mode**: Always fix compile errors before testing
- **Missing references**: Unity serialized references can break if files are renamed/moved - use Unity's refactoring tools
- **Singleton patterns**: GameManagerComponent uses singleton pattern (`GameManagerComponent.Instance`) - ensure it exists in scene
- **StatComponent required**: Many systems expect GameObjects to have a StatComponent - verify dependencies

### Known Issues and Patterns
- **Boss Scaling**: Automatically applied via `BossScalingSystem` component on boss prefabs (see SCALING_SYSTEM.md)
- **ATK Stat Propagation**: Fixed in RingAttack and ClusterAttack to properly copy attack context
- **Round Progression**: Managed by `GameManagerComponent.HandleOnBossRoomState()`

## Validation Checklist

When making changes, always:
1. Verify scripts compile without errors
2. Test in Unity Play Mode
3. Check Unity Console for warnings/errors
4. If modifying stats: Verify StatComponent and StatMediator interactions
5. If modifying combat: Test damage calculations with different ATK values
6. If modifying boss systems: Test across multiple rounds to verify scaling
7. If adding new components: Ensure they follow existing MonoBehaviour patterns

## Additional Context

### Documentation
- **README.md**: Basic project info and build file links
- **SCALING_SYSTEM.md**: Detailed documentation of boss scaling system and ATK stat bug fix

### External Resources
- Build files are hosted on Google Drive (link in README.md)
- Demo video available on YouTube (link in README.md)

## Instructions for Coding Agent

**Trust these instructions.** Only search the codebase when:
- The information above is incomplete or outdated
- You need specific implementation details not covered here
- You encounter unexpected errors or behavior

**When working with this project:**
- This is a Unity project - all builds and tests are done through Unity Editor, not command line
- Focus changes in `Assets/Scripts/` directory
- Respect the existing component-based architecture
- Use the StatMediator for any stat modifications
- Test changes manually in Unity Play Mode
- Check for null references - Unity's serialization can create null reference issues
- When modifying damage/combat: Always test with different ATK values to verify stat application

**Before completing your task:**
- Ensure all scripts compile
- Test your changes in Unity Play Mode
- Document any new systems in markdown format similar to SCALING_SYSTEM.md
- Verify no unintended side effects in related systems
