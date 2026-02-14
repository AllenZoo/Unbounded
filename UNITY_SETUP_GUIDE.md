# High Score System - Unity Editor Setup Guide

This guide provides step-by-step instructions for setting up the High Score & Run History System in the Unity Editor.

## Prerequisites

All code files have been created. This guide covers the Unity Editor asset creation and scene setup.

## Step 1: Create ScriptableObject Assets

### Create HighScoreContext Asset

1. In the Unity Project window, navigate to `Assets/UI Toolkit/Data/HighScoreOverlay/`
2. Right-click in the folder → Create → System → Contexts → High Score Context
3. Name the asset: **HighScoreContext**
4. Leave all settings at their defaults

### Create HighScoreUIData Asset

1. In the same folder (`Assets/UI Toolkit/Data/HighScoreOverlay/`)
2. Right-click → Create → System → UI Toolkit → High Score Data
3. Name the asset: **HighScoreUIData**
4. Leave all settings at their defaults

## Step 2: Add RunHistoryManager to Persistent Scene

1. Open the **Persistent** scene (or whichever scene persists across game sessions)
2. Find or create a GameObject for managers (e.g., "GameManagers" or "Systems")
3. Add Component → Search for **RunHistoryManager**
4. The component requires no configuration - it auto-registers with the data persistence system

**Verify**: The component should appear with fields showing:
- Current High Score: 0
- Run History: (empty list)

## Step 3: Setup High Score UI

### Option A: Add to Existing UI Scene

If you have a UI overlay scene:

1. Open your UI overlay scene
2. Create a new GameObject, name it **HighScoreUI**
3. Add Component → UI → UI Document
4. In the UI Document component:
   - Panel Settings: Use your existing PanelSettings (likely at `Assets/UI Toolkit/PanelSettings.asset`)
   - Source Asset: Drag `Assets/UI Toolkit/HighScoreUI.uxml`
5. Add Component → Search for **HighScoreController**
6. In the HighScoreController inspector:
   - **High Score UI Document**: Drag the UIDocument component (on the same GameObject)
   - **High Score Context**: Drag `Assets/UI Toolkit/Data/HighScoreOverlay/HighScoreContext.asset`
   - **High Score UI Data**: Drag `Assets/UI Toolkit/Data/HighScoreOverlay/HighScoreUIData.asset`
   - **Main Menu Scene**: Drag your main menu scene reference

### Option B: Create a New Scene

If you want a dedicated high score scene:

1. Create a new scene: File → New Scene
2. Name it **HighScoreScene**
3. Follow steps 2-6 from Option A above
4. Save the scene to `Assets/Scenes/HighScoreScene.unity`

## Step 4: Add High Score Button to Main Menu

### For UI Toolkit Menu

If your main menu uses UI Toolkit:

1. Open your main menu UXML file
2. Add a new button element:
   ```xml
   <ui:Button text="High Scores" name="HighScoresButton" />
   ```
3. In your main menu controller script, add:
   ```csharp
   [SerializeField] private HighScoreContext highScoreContext;
   
   private void Start()
   {
       var highScoresButton = rootVisualElement.Q<Button>("HighScoresButton");
       highScoresButton.RegisterCallback<ClickEvent>(OnHighScoresClicked);
   }
   
   private void OnHighScoresClicked(ClickEvent evt)
   {
       highScoreContext.Open();
   }
   ```
4. In the inspector, assign the HighScoreContext asset

### For uGUI Canvas Menu

If your main menu uses Unity UI (Canvas):

1. Create a new Button in your main menu Canvas
2. Set the button text to "High Scores"
3. Create a GameObject in the scene, name it **HighScoreOpener**
4. Add Component → Search for **HighScoreOpener**
5. In the HighScoreOpener inspector:
   - **High Score Context**: Drag `Assets/UI Toolkit/Data/HighScoreOverlay/HighScoreContext.asset`
6. Select your High Scores button
7. In the Button component, add a new OnClick() event:
   - Drag the HighScoreOpener GameObject to the object field
   - Select: HighScoreOpener → OpenHighScoreScreen()

## Step 5: Test the System

### Initial Testing

1. Enter Play Mode
2. Start a new game
3. Play through at least one run until game over
4. Return to main menu
5. Click the "High Scores" button
6. Verify the UI appears and shows your run

### Persistence Testing

1. Complete 2-3 runs in Play Mode
2. Exit Play Mode
3. Re-enter Play Mode
4. Open High Scores
5. Verify your previous runs are still displayed

### Weapon Tracking Testing

1. Start a new run
2. Equip different weapons during the run
3. Complete the run
4. Open High Scores
5. Verify the run shows the weapons you used

## Troubleshooting

### "HighScoreContainer not found" error

- Check that HighScoreUI.uxml is assigned correctly in the UIDocument
- Verify the UXML file has a VisualElement with name="HighScoreContainer"

### High score not persisting

- Verify RunHistoryManager is in the Persistent scene
- Check Console for data persistence errors
- Ensure DataPersistenceHandler is properly configured

### Weapons not being tracked

- Check that inventory modifications are firing events
- Verify the Player has a tag "Player"
- Check Console for weapon tracking logs

### UI not opening

- Verify HighScoreContext is assigned in both the HighScoreController and the button handler
- Check Console for errors when clicking the button
- Ensure the high score UI GameObject is active in the scene

## Optional Enhancements

### Styling the UI

Edit `Assets/UI Toolkit/styles.uss` to customize:
- `.run-entry` - Entry background and borders
- `.run-score` - Score text color and size
- `.run-timestamp` - Timestamp styling
- `.run-weapons` - Weapon list appearance

### Adding More Statistics

Edit `HighScoreUIData.cs` to add more formatted fields:
1. Add public string fields for new data
2. Update `UpdateFromRunHistory()` to populate them
3. Modify `CreateRunEntry()` in HighScoreController.cs to display them

## Verification Checklist

- [ ] HighScoreContext asset created
- [ ] HighScoreUIData asset created
- [ ] RunHistoryManager added to persistent scene
- [ ] High Score UI GameObject created with UIDocument
- [ ] HighScoreController configured with all references
- [ ] High Scores button added to main menu
- [ ] Button wired to open high score screen
- [ ] Tested in Play Mode - runs are recorded
- [ ] Tested persistence - data survives scene changes
- [ ] Verified weapon tracking works

## Next Steps

Once setup is complete, see `HIGH_SCORE_SYSTEM.md` for:
- System architecture details
- Integration points
- Future enhancement ideas
