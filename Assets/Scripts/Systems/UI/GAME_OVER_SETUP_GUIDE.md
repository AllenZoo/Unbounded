# Quick Start Guide: Integrating the Game Over UI

This guide shows you how to integrate the Game Over Score Summary UI into your Unity scene.

## Prerequisites

- Unity Editor (the system has been implemented in the codebase)
- Basic familiarity with Unity UI and Canvas system
- TextMeshPro package installed (should already be in the project)

## Step-by-Step Setup

### 1. Create the Canvas (if not already present)

1. In the Unity Hierarchy, right-click and select `UI > Canvas`
2. Name it something like "GameOverCanvas"
3. Set Canvas properties:
   - Render Mode: Screen Space - Overlay (or your preference)
   - Canvas Scaler: Scale with Screen Size (recommended)

### 2. Create the Game Over UI GameObject

1. Right-click on your Canvas and select `Create Empty`
2. Name it "GameOverPanel"
3. Add components:
   - Add `Canvas` component (will be used for sorting)
   - Add `Box Collider 2D` component (required by PageUI)
     - Set to trigger: ✓ Is Trigger
     - Adjust size to cover the full screen
   - Add `GameOverUI` script component

### 3. Create the UI Layout

Create child TextMeshPro elements under GameOverPanel:

#### Background (Optional but recommended)
1. Add `UI > Image` as child
2. Name it "Background"
3. Stretch to fill parent (Anchor: stretch-stretch)
4. Set color to semi-transparent black (e.g., RGBA: 0, 0, 0, 200)

#### Score Display
1. Add `UI > TextMeshProUGUI` elements with these names:
   - "TotalScoreText"
   - "DamageScoreText"
   - "TimeScoreText"
   - "BossesDefeatedText"

2. Position them vertically in the center-top area

#### Statistics Display
1. Add more `UI > TextMeshProUGUI` elements:
   - "TotalDamageText"
   - "TimeSurvivedText"

2. Position them in the middle area

#### Buttons (Optional)
1. Add `UI > Button - TextMeshPro`:
   - "RetryButton"
   - "MainMenuButton"

2. Position them at the bottom

### 4. Configure the GameOverUI Component

Select the GameOverPanel GameObject and in the Inspector:

#### Canvas & Collider References
- Drag the Canvas component to the `canvas` field
- Drag the Box Collider 2D to the `uiCollider` field

#### Score Display References
- Drag "TotalScoreText" to `Total Score Text`
- Drag "DamageScoreText" to `Damage Score Text`
- Drag "TimeScoreText" to `Time Score Text`
- Drag "BossesDefeatedText" to `Bosses Defeated Text`

#### Statistics Display References
- Drag "TotalDamageText" to `Total Damage Text`
- Drag "TimeSurvivedText" to `Time Survived Text`

#### Button References (Optional)
- Drag "RetryButton" to `Retry Button` (if you created it)
- Drag "MainMenuButton" to `Main Menu Button` (if you created it)

#### PageUIContext
- Assign `GameOverPage.asset` (located in `Assets/Scripts/Systems/UI/Page/`)

### 5. Configure GameObject Layer

1. Select the GameOverPanel GameObject
2. Set Layer to "UI" (important for collision detection)
3. When prompted to set children to the same layer, choose "Yes"

### 6. Test the Setup

#### Test in Play Mode:
1. Press Play in Unity Editor
2. The Game Over panel should be hidden initially
3. When the player dies, the panel should automatically appear with score data

#### Manual Test (if needed):
```csharp
// You can trigger the Game Over UI manually for testing
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

## Example Layout Structure

```
Canvas
└── GameOverPanel (GameOverUI component)
    ├── Background (Image)
    ├── TitleText ("GAME OVER")
    ├── ScoreSection
    │   ├── TotalScoreText
    │   ├── DamageScoreText
    │   ├── TimeScoreText
    │   └── BossesDefeatedText
    ├── StatsSection
    │   ├── TotalDamageText
    │   └── TimeSurvivedText
    └── ButtonSection
        ├── RetryButton
        └── MainMenuButton
```

## Styling Tips

### Font Sizes (Recommended)
- Title: 48-72pt
- Total Score: 36-48pt
- Score Breakdown: 24-30pt
- Statistics: 20-24pt
- Buttons: 24-30pt

### Colors (Suggested)
- Total Score: Gold/Yellow (#FFD700)
- Positive scores: Green (#00FF00)
- Negative scores: Red (#FF0000)
- Statistics: White/Light Gray (#CCCCCC)

### Layout
- Use anchors for responsive design
- Center important information
- Leave adequate padding (20-40px)
- Use vertical layout groups for automatic spacing

## Troubleshooting

### Panel doesn't appear on player death
- Check that GameManagerComponent is calling `OnPlayerDeath()`
- Verify EventBus is working (check Debug logs)
- Ensure Canvas is enabled

### UI Elements not assigned
- Make sure all TextMeshPro references are dragged in the Inspector
- Check for null reference warnings in Console

### UI appears but is behind other elements
- Adjust Canvas sorting order
- Check UIOverlayManager is present in the scene
- Verify the UI layer is correct

### Buttons don't work
- Ensure buttons have Event System in the scene
- Check button listeners are assigned in GameOverUI
- Verify button GameObjects are active

## Advanced Customization

### Adding Animations
```csharp
// Override in a subclass
protected override void OnEnable()
{
    base.OnEnable();
    // Add fade-in or slide-in animation
    LeanTween.alpha(gameObject.GetComponent<RectTransform>(), 1f, 0.5f).setFrom(0f);
}
```

### Adding Sound Effects
```csharp
private void OnGameOver(OnGameOverEvent e)
{
    // Play game over sound
    AudioManager.Instance?.PlaySound("GameOverSound");
    
    DisplayScoreSummary(e.scoreSummary);
    MoveToTop();
}
```

### Custom Score Display
```csharp
public void DisplayScoreSummary(ScoreSummaryData data)
{
    // Animate score counting up
    StartCoroutine(AnimateScore(data.totalScore));
    
    // ... rest of display logic
}

private IEnumerator AnimateScore(int targetScore)
{
    int currentScore = 0;
    float duration = 2f;
    float elapsed = 0f;
    
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        currentScore = Mathf.RoundToInt(Mathf.Lerp(0, targetScore, elapsed / duration));
        totalScoreText.text = $"Final Score: {currentScore:N0}";
        yield return null;
    }
    
    totalScoreText.text = $"Final Score: {targetScore:N0}";
}
```

## Next Steps

After basic setup:
1. Style the UI to match your game's aesthetic
2. Add animations for polish
3. Implement high score tracking
4. Add reward/unlock displays
5. Integrate with analytics

For more details, see `GAME_OVER_SYSTEM.md`
