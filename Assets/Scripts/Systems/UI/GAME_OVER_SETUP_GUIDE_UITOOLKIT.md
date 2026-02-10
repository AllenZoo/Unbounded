# Quick Start Guide: Integrating the Game Over UI (UI Toolkit)

This guide shows you how to integrate the Game Over Score Summary UI into your Unity scene using **Unity's UI Toolkit**.

## Prerequisites

- Unity Editor (the system has been implemented in the codebase)
- Basic familiarity with Unity UI Toolkit (formerly UI Elements)
- UI Toolkit package installed (should already be in the project)

## Step-by-Step Setup

### 1. Create the UI Document GameObject

1. In the Unity Hierarchy, right-click and select `UI Toolkit > UI Document`
2. Name it "GameOverUIDocument"
3. In the Inspector, configure the UI Document component:
   - **Source Asset**: Assign `Assets/UI Toolkit/GameOverUI.uxml`
   - **Panel Settings**: Assign the project's PanelSettings asset (usually `Assets/UI Toolkit/PanelSettings.asset`)
   - **Sort Order**: Set to a high value (e.g., 100) to ensure it appears on top

### 2. Create ScriptableObject Assets

#### Create GameOverContext
1. In Project window, navigate to `Assets/UI Toolkit/Data/`
2. Right-click > `Create > System > Contexts > Game Over Context`
3. Name it `GameOverContext`

#### Create GameOverUIData
1. In Project window, navigate to `Assets/UI Toolkit/Data/`
2. Right-click > `Create > ScriptableObject` (you may need to create a custom menu option)
3. Create an instance of `GameOverUIData`
4. Name it `GameOverUIData`

### 3. Add the GameOverController Component

1. Select the "GameOverUIDocument" GameObject
2. In the Inspector, click `Add Component`
3. Search for and add `GameOverController`
4. Configure the component:
   - **Game Over UI Document**: Drag the UI Document component (or the GameObject itself)
   - **Game Over Context**: Assign the `GameOverContext` asset you created
   - **Game Over UI Data**: Assign the `GameOverUIData` asset you created

### 4. Test the Setup

#### Test in Play Mode:
1. Press Play in Unity Editor
2. The Game Over UI should be hidden initially
3. When the player dies, the UI should automatically appear with score data

#### Manual Test:
You can trigger the Game Over UI manually for testing:

1. Add the `GameOverUITester` component to any GameObject in your scene
2. Press Play
3. Press `F9` to trigger a test game over with sample data
4. Press `F10` to trigger a test game over with zero score

Or use the following code in the Console or a test script:

```csharp
// Trigger manually
ScoreSummaryData testData = new ScoreSummaryData
{
    totalScore = 12345,
    damageScore = 10000,
    timeScore = 2345,
    bossesDefeated = 3,
    totalDamageDealt = 10000f,
    totalTimeSurvived = 185.5f
};

EventBus<OnGameOverEvent>.Raise(new OnGameOverEvent { scoreSummary = testData });
```

## UI Customization

### Modifying the Layout

To customize the Game Over UI layout:

1. Open `Assets/UI Toolkit/GameOverUI.uxml` in the UI Builder
2. Modify the visual elements, text sizes, colors, and layout
3. Save the changes

### Modifying Styles

To customize the visual appearance:

1. Open `Assets/UI Toolkit/styles.uss` (or create a custom stylesheet)
2. Add or modify USS classes and styles
3. Apply the styles to elements in the UXML file

### Example Style Customizations

Add to your USS file:

```css
.game-over-title {
    font-size: 72px;
    color: rgb(255, 215, 0);
    -unity-font-style: bold;
}

.score-label {
    font-size: 24px;
    color: rgb(255, 255, 255);
    margin-bottom: 5px;
}

.game-over-button {
    height: 80px;
    width: 200px;
    border-radius: 12px;
    font-size: 28px;
}
```

## Architecture Overview

The UI Toolkit implementation follows this pattern:

```
OnGameOverEvent (triggered by GameManagerComponent)
    ↓
GameOverController (receives event)
    ↓
Updates GameOverUIData (formats data for display)
    ↓
Opens GameOverContext (signals UI to show)
    ↓
UI Document displays via data binding
```

## Key Components

### GameOverController
- Listens to `OnGameOverEvent`
- Manages UI Document visibility
- Handles button clicks (Retry, Main Menu)
- Updates data binding when score changes

### GameOverContext
- ScriptableObject that holds UI state (open/closed)
- Bridges game logic and UI display
- Notifies controller of state changes

### GameOverUIData
- ScriptableObject for data binding
- Converts `ScoreSummaryData` to formatted strings
- Automatically updates UI when properties change

### GameOverUI.uxml
- Defines the visual structure of the Game Over screen
- Uses data binding to display score information
- Supports responsive layout

## Troubleshooting

### UI doesn't appear on player death
- Check that `GameOverController` is in the scene
- Verify `GameOverContext` and `GameOverUIData` are assigned
- Check Debug logs for error messages
- Ensure `UIDocument` has the correct UXML assigned

### Data binding not working
- Verify `GameOverUIData` is assigned to the UI Document's dataSource
- Check that property names in UXML match property names in `GameOverUIData`
- Ensure binding-mode is set to "ToTarget"

### Buttons don't respond
- Check that button names in UXML match the queries in `GameOverController`
- Verify ClickEvent callbacks are registered
- Check for null references in Debug logs

### UI appears behind other elements
- Increase the Sort Order in the UI Document component
- Check Panel Settings configuration
- Verify the UI Document is not disabled

## Advanced: Custom Data Binding

To add new data fields:

1. Add a public property to `GameOverUIData.cs`:
```csharp
public string newStatText;
```

2. Update the property in `UpdateFromScoreSummary()`:
```csharp
newStatText = $"New Stat: {data.newStat}";
```

3. Add a Label to `GameOverUI.uxml` with data binding:
```xml
<ui:Label text="New Stat: 0" name="NewStatLabel">
    <Bindings>
        <ui:DataBinding property="text" data-source-path="newStatText" binding-mode="ToTarget"/>
    </Bindings>
</ui:Label>
```

## Benefits of UI Toolkit Implementation

- **Performance**: UI Toolkit is more performant than Canvas UI
- **Flexibility**: Easy to modify layout without touching code
- **Data Binding**: Automatic UI updates when data changes
- **Styling**: CSS-like styling with USS
- **Responsive**: Better support for different screen sizes
- **Editor Tools**: UI Builder for visual editing

For more details on the system architecture, see `GAME_OVER_SYSTEM.md`
