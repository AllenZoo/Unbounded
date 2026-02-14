using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controller for the High Score UI using UI Toolkit.
/// Displays high score and run history with weapon details.
/// </summary>
public class HighScoreController : MonoBehaviour
{
    [FoldoutGroup("UI Fields")]
    [Required, SerializeField] private UIDocument highScoreUIDocument;

    [FoldoutGroup("UI Fields")]
    [Required, SerializeField] private HighScoreContext highScoreContext;

    [FoldoutGroup("UI Fields")]
    [Required, SerializeField] private HighScoreUIData highScoreUIData;

    [FoldoutGroup("Scenes")]
    [Required, SerializeField] private SceneField mainMenuScene;

    private VisualElement rootContainer;
    private VisualElement backButton;
    private VisualElement highScoreLabel;
    private ScrollView runHistoryScrollView;

    private void Start()
    {
        // Get root container from UI Document
        rootContainer = highScoreUIDocument.rootVisualElement.Q<VisualElement>("HighScoreContainer");
        
        if (rootContainer == null)
        {
            Debug.LogError("HighScoreController: Could not find HighScoreContainer in UI Document");
            return;
        }

        // Get UI elements
        backButton = rootContainer.Q<VisualElement>("BackButton");
        highScoreLabel = rootContainer.Q<VisualElement>("HighScoreLabel");
        runHistoryScrollView = rootContainer.Q<ScrollView>("RunHistoryScrollView");

        // Register button callbacks
        if (backButton != null)
        {
            backButton.RegisterCallback<ClickEvent>(OnBackClicked);
        }
        else
        {
            Debug.LogWarning("HighScoreController: BackButton not found");
        }

        // Set data source for data binding
        rootContainer.dataSource = highScoreUIData;

        // Start hidden
        rootContainer.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        if (highScoreContext != null)
        {
            highScoreContext.OnChanged += HandleHighScoreContextChanged;
        }
    }

    private void OnDisable()
    {
        if (highScoreContext != null)
        {
            highScoreContext.OnChanged -= HandleHighScoreContextChanged;
        }
    }

    /// <summary>
    /// Handles changes to the HighScoreContext.
    /// </summary>
    private void HandleHighScoreContextChanged()
    {
        if (rootContainer == null) return;

        if (highScoreContext.IsOpen)
        {
            // Update the data before showing
            highScoreUIData.UpdateFromRunHistory();
            
            // Rebuild the run history list
            RebuildRunHistoryList();

            rootContainer.style.display = DisplayStyle.Flex;
            Debug.Log("HighScoreController: Showing High Score UI");
        }
        else
        {
            rootContainer.style.display = DisplayStyle.None;
            Debug.Log("HighScoreController: Hiding High Score UI");
        }
    }

    /// <summary>
    /// Rebuilds the run history list in the UI.
    /// </summary>
    private void RebuildRunHistoryList()
    {
        if (runHistoryScrollView == null)
        {
            Debug.LogWarning("HighScoreController: RunHistoryScrollView not found");
            return;
        }

        // Clear existing entries
        runHistoryScrollView.Clear();

        // Add each run entry
        foreach (var runData in highScoreUIData.runHistoryList)
        {
            VisualElement runEntry = CreateRunEntry(runData);
            runHistoryScrollView.Add(runEntry);
        }

        Debug.Log($"HighScoreController: Rebuilt list with {highScoreUIData.runHistoryList.Count} entries");
    }

    /// <summary>
    /// Creates a visual element for a single run entry.
    /// </summary>
    private VisualElement CreateRunEntry(RunHistoryDisplayData runData)
    {
        VisualElement entry = new VisualElement();
        entry.AddToClassList("run-entry");

        // Create header with score and timestamp
        VisualElement header = new VisualElement();
        header.AddToClassList("run-entry-header");

        Label scoreLabel = new Label(runData.scoreText);
        scoreLabel.AddToClassList("run-score");
        header.Add(scoreLabel);

        Label timestampLabel = new Label(runData.timestamp);
        timestampLabel.AddToClassList("run-timestamp");
        header.Add(timestampLabel);

        entry.Add(header);

        // Create stats section
        VisualElement statsSection = new VisualElement();
        statsSection.AddToClassList("run-stats");

        Label bossesLabel = new Label(runData.bossesDefeatedText);
        statsSection.Add(bossesLabel);

        Label damageLabel = new Label(runData.damageText);
        statsSection.Add(damageLabel);

        Label durationLabel = new Label(runData.durationText);
        statsSection.Add(durationLabel);

        entry.Add(statsSection);

        // Create weapons section
        Label weaponsLabel = new Label($"Weapons: {runData.weaponsText}");
        weaponsLabel.AddToClassList("run-weapons");
        entry.Add(weaponsLabel);

        return entry;
    }

    /// <summary>
    /// Handles back button click.
    /// </summary>
    private void OnBackClicked(ClickEvent evt)
    {
        Debug.Log("HighScoreController: Back button clicked");
        
        // Close the High Score UI
        highScoreContext.Close();

        // Return to main menu
        EventBus<OnSceneLoadRequest>.Call(new OnSceneLoadRequest
        {
            scenesToLoad = new List<SceneField> { mainMenuScene },
            scenesToUnload = new List<SceneField> { },
            activeSceneToSet = mainMenuScene,
            showLoadingBar = true,
            unloadAllButPersistent = false
        });
    }

    private void OnDestroy()
    {
        // Unregister button callbacks
        if (backButton != null)
        {
            backButton.UnregisterCallback<ClickEvent>(OnBackClicked);
        }
    }

    /// <summary>
    /// Public method to open the high score screen.
    /// Can be called from other UI elements or buttons.
    /// </summary>
    public void OpenHighScoreScreen()
    {
        highScoreContext.Open();
    }
}
