# High Score System Architecture Diagram

## System Flow

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                          HIGH SCORE SYSTEM OVERVIEW                          │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│                               GAME FLOW                                      │
└─────────────────────────────────────────────────────────────────────────────┘

    Game Start
         │
         ├──> GameManagerComponent.StartNewRun()
         │         │
         │         └──> RunHistoryManager.StartNewRun()
         │                   │
         │                   └──> Clear weapons, reset tracking
         │
    Player Equips Weapon
         │
         ├──> InventorySystem modifies
         │         │
         │         └──> OnInventoryModifiedEvent fired
         │                   │
         │                   └──> RunHistoryManager.HandleInventoryModified()
         │                            │
         │                            └──> Track unique weapon
         │
    Player Dies / Run Ends
         │
         ├──> GameManagerComponent.OnPlayerDeath()
         │         │
         │         ├──> RunTracker.EndRun() → ScoreSummaryData
         │         │
         │         └──> OnGameOverEvent fired with ScoreSummaryData
         │                   │
         │                   └──> RunHistoryManager.HandleGameOver()
         │                            │
         │                            ├──> Create RunHistoryData
         │                            ├──> Update high score if needed
         │                            ├──> Add to circular buffer
         │                            └──> DataPersistenceHandler.SaveGame()
         │
    User Opens High Score Screen
         │
         ├──> Button Click / Menu Selection
         │         │
         │         └──> HighScoreContext.Open()
         │                   │
         │                   └──> OnChanged event fired
         │                            │
         │                            └──> HighScoreController.HandleContextChanged()
         │                                     │
         │                                     ├──> HighScoreUIData.UpdateFromRunHistory()
         │                                     │         │
         │                                     │         └──> Format data from RunHistoryManager
         │                                     │
         │                                     └──> Rebuild UI with run entries


┌─────────────────────────────────────────────────────────────────────────────┐
│                         DATA PERSISTENCE FLOW                                │
└─────────────────────────────────────────────────────────────────────────────┘

    Game Loads
         │
         └──> DataPersistenceHandler.LoadGame()
                   │
                   ├──> Load GameData from file
                   │         │
                   │         ├──> highScore (int)
                   │         └──> runHistory (List<RunHistoryData>)
                   │
                   └──> Call IDataPersistence.LoadData() on all components
                            │
                            └──> RunHistoryManager.LoadData(GameData data)
                                     │
                                     ├──> currentHighScore = data.highScore
                                     └──> runHistory = data.runHistory

    Game Saves (on run end)
         │
         └──> DataPersistenceHandler.SaveGame()
                   │
                   ├──> Create GameData object
                   │
                   ├──> Call IDataPersistence.SaveData() on all components
                   │         │
                   │         └──> RunHistoryManager.SaveData(GameData data)
                   │                   │
                   │                   ├──> data.highScore = currentHighScore
                   │                   └──> data.runHistory = runHistory
                   │
                   └──> Serialize and write to file


┌─────────────────────────────────────────────────────────────────────────────┐
│                           COMPONENT DIAGRAM                                  │
└─────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────┐
│   GameManagerComponent   │  (Existing)
│  ─────────────────────── │
│  + StartNewRun()         │──────┐
│  + OnPlayerDeath()       │      │
└──────────────────────────┘      │
                                   │
                                   ▼
┌──────────────────────────┐  ┌─────────────────────────┐
│   RunHistoryManager      │  │     GameData            │  (Modified)
│  ─────────────────────── │  │  ────────────────────── │
│  + StartNewRun()         │  │  + highScore: int       │
│  + HandleGameOver()      │◄─┤  + runHistory: List<..> │
│  + LoadData()            │  └─────────────────────────┘
│  + SaveData()            │
│  - TrackCurrentWeapon()  │
│  - cachedPlayer          │
│  - cachedInventory       │
└──────────────────────────┘
          │
          ├──────────────┐
          │              │
          ▼              ▼
┌──────────────────┐  ┌──────────────────┐
│ RunHistoryData   │  │ WeaponUsageData  │
│ ──────────────── │  │ ──────────────── │
│ + score          │  │ + weaponId       │
│ + weaponsUsed    │  │ + weaponName     │
│ + duration       │  │ + equipTime      │
│ + timestamp      │  └──────────────────┘
│ + bossesDefeated │
└──────────────────┘

┌──────────────────────────┐
│  HighScoreController     │
│  ─────────────────────── │
│  + HandleContextChanged()│
│  + RebuildRunHistory()   │
│  - CreateRunEntry()      │
└──────────────────────────┘
          │
          ├──────────────┬─────────────┐
          ▼              ▼             ▼
┌──────────────────┐  ┌─────────────┐  ┌──────────────────┐
│ HighScoreContext │  │HighScoreUI  │  │HighScoreUIData   │
│ ──────────────── │  │  Data       │  │ ──────────────── │
│ + IsOpen         │  │ ─────────── │  │ + highScoreText  │
│ + OnChanged      │  │ + uxml      │  │ + runHistoryList │
│ + Open()         │  │ + styles    │  │ + UpdateFrom...  │
│ + Close()        │  └─────────────┘  └──────────────────┘
└──────────────────┘


┌─────────────────────────────────────────────────────────────────────────────┐
│                           EVENT BUS FLOW                                     │
└─────────────────────────────────────────────────────────────────────────────┘

                         EventBus<OnGameOverEvent>
                                    │
                   ┌────────────────┴────────────────┐
                   ▼                                 ▼
        RunHistoryManager                  GameOverController
        (Record run data)                  (Show game over UI)


                    EventBus<OnInventoryModifiedEvent>
                                    │
                                    ▼
                          RunHistoryManager
                          (Track weapons)


┌─────────────────────────────────────────────────────────────────────────────┐
│                            UI STRUCTURE                                      │
└─────────────────────────────────────────────────────────────────────────────┘

HighScoreUI.uxml
│
└─ HighScoreContainer (VisualElement)
   │
   └─ HighScorePanel (VisualElement)
      │
      ├─ TitleLabel ("HIGH SCORES")
      │
      ├─ HighScoreLabel (data-bound to highScoreText)
      │
      ├─ HistoryTitleLabel ("Run History")
      │
      ├─ RunHistoryScrollView (ScrollView)
      │  │
      │  └─ [Dynamically generated run entries]
      │     │
      │     └─ RunEntry (VisualElement)
      │        │
      │        ├─ RunEntryHeader
      │        │  ├─ ScoreLabel
      │        │  └─ TimestampLabel
      │        │
      │        ├─ RunStats
      │        │  ├─ BossesLabel
      │        │  ├─ DamageLabel
      │        │  └─ DurationLabel
      │        │
      │        └─ WeaponsLabel
      │
      └─ ButtonSection
         │
         └─ BackButton


┌─────────────────────────────────────────────────────────────────────────────┐
│                      CIRCULAR BUFFER LOGIC                                   │
└─────────────────────────────────────────────────────────────────────────────┘

runHistory (List<RunHistoryData>)
MAX_RUN_HISTORY = 50

On new run completion:
    1. Create RunHistoryData
    2. Add to runHistory list
    3. If runHistory.Count > MAX_RUN_HISTORY:
          runHistory.RemoveAt(0)  // Remove oldest

Display order: Reverse (newest first)


┌─────────────────────────────────────────────────────────────────────────────┐
│                      FILE ORGANIZATION                                       │
└─────────────────────────────────────────────────────────────────────────────┘

Assets/
├── Scripts/
│   └── Systems/
│       ├── Scoring/
│       │   ├── RunHistoryData.cs         ← Data structures
│       │   ├── RunHistoryManager.cs      ← Manager component
│       │   └── HIGH_SCORE_SYSTEM.md      ← Architecture docs
│       │
│       ├── DataPersistence/
│       │   └── GameData.cs               ← Modified (added fields)
│       │
│       ├── GameManagment/
│       │   └── GameManagerComponent.cs   ← Modified (added hook)
│       │
│       └── UI/
│           └── HighScoreOpener.cs        ← Helper component
│
└── UI Toolkit/
    ├── HighScoreUI.uxml                  ← UI layout
    ├── styles.uss                        ← Modified (added styles)
    │
    ├── Controllers/
    │   └── HighScoreController.cs        ← UI controller
    │
    └── Data/
        └── HighScoreOverlay/
            ├── HighScoreContext.cs       ← State management
            └── HighScoreUIData.cs        ← Data binding


┌─────────────────────────────────────────────────────────────────────────────┐
│                         KEY DESIGN PRINCIPLES                                │
└─────────────────────────────────────────────────────────────────────────────┘

1. EVENT-DRIVEN
   ├── Loose coupling between components
   ├── Uses EventBus for communication
   └── Easy to extend and maintain

2. DATA BINDING
   ├── ScriptableObjects for state
   ├── UI Toolkit data binding
   └── Reactive UI updates

3. SEPARATION OF CONCERNS
   ├── Data (RunHistoryData)
   ├── Logic (RunHistoryManager)
   ├── Presentation (HighScoreController)
   └── State (HighScoreContext)

4. PERFORMANCE
   ├── Cached references
   ├── No FindObjectOfType in hot paths
   ├── Efficient data structures
   └── Circular buffer (bounded memory)

5. PERSISTENCE
   ├── IDataPersistence interface
   ├── Integrates with existing save system
   ├── Automatic save on game over
   └── JSON serialization

6. MINIMAL CHANGES
   ├── Only 3 existing files modified
   ├── No breaking changes
   ├── Follows existing patterns
   └── Clean integration
```
