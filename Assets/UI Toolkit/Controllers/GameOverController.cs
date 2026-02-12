using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controller for the Game Over UI using UI Toolkit.
/// Listens to game over events and displays the score summary using UI Toolkit.
/// </summary>
public class GameOverController : MonoBehaviour
{
    [FoldoutGroup("UI Fields")]
    [Required, SerializeField] private UIDocument gameOverUIDocument;

    [FoldoutGroup("UI Fields")]
    [Required, SerializeField] private GameOverContext gameOverContext;

    [FoldoutGroup("UI Fields")]
    [Required, SerializeField] private GameOverUIData gameOverUIData;


    [FoldoutGroup("Scenes")]
    [Required, SerializeField] private SceneField mainMenuScene;

    [FoldoutGroup("Scenes")]
    [Required, SerializeField] private SceneField anchorPointScene;

    [FoldoutGroup("Scenes")]
    [Required, SerializeField] private SceneField gameOverScene;


    private VisualElement rootContainer;
    private VisualElement retryButton;
    private VisualElement mainMenuButton;
    
    private EventBinding<OnGameOverEvent> gameOverBinding;

    private void Awake()
    {
        // Register for game over event
        gameOverBinding = new EventBinding<OnGameOverEvent>(OnGameOver);
        EventBus<OnGameOverEvent>.Register(gameOverBinding);
    }

    private void Start()
    {
        // Get root container from UI Document
        rootContainer = gameOverUIDocument.rootVisualElement.Q<VisualElement>("GameOverContainer");
        
        if (rootContainer == null)
        {
            Debug.LogError("GameOverController: Could not find GameOverContainer in UI Document");
            return;
        }

        // Get buttons
        retryButton = rootContainer.Q<VisualElement>("RetryButton");
        mainMenuButton = rootContainer.Q<VisualElement>("MainMenuButton");

        // Register button callbacks
        if (retryButton != null)
        {
            retryButton.RegisterCallback<ClickEvent>(OnRetryClicked);
        }
        else
        {
            Debug.LogWarning("GameOverController: RetryButton not found");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.RegisterCallback<ClickEvent>(OnMainMenuClicked);
        }
        else
        {
            Debug.LogWarning("GameOverController: MainMenuButton not found");
        }

        // Set data source for data binding
        rootContainer.dataSource = gameOverUIData;

        // Start hidden
        rootContainer.style.display = DisplayStyle.None;
    }

    private void OnEnable()
    {
        if (gameOverContext != null)
        {
            gameOverContext.OnChanged += HandleGameOverContextChanged;
        }
    }

    private void OnDisable()
    {
        if (gameOverContext != null)
        {
            gameOverContext.OnChanged -= HandleGameOverContextChanged;
        }
    }

    /// <summary>
    /// Handles the OnGameOverEvent from the event bus.
    /// </summary>
    private void OnGameOver(OnGameOverEvent e)
    {
        if (e.scoreSummary == null)
        {
            Debug.LogWarning("GameOverController: Received OnGameOverEvent with null scoreSummary");
            return;
        }

        // Update the UI data
        gameOverUIData.UpdateFromScoreSummary(e.scoreSummary);
        
        // Open the context
        gameOverContext.Open(e.scoreSummary);
    }

    /// <summary>
    /// Handles changes to the GameOverContext.
    /// </summary>
    private void HandleGameOverContextChanged()
    {
        if (rootContainer == null) return;

        if (gameOverContext.IsOpen)
        {
            rootContainer.style.display = DisplayStyle.Flex;
            Debug.Log("GameOverController: Showing Game Over UI");
        }
        else
        {
            rootContainer.style.display = DisplayStyle.None;
            Debug.Log("GameOverController: Hiding Game Over UI");
        }
    }

    /// <summary>
    /// Handles retry button click.
    /// </summary>
    private void OnRetryClicked(ClickEvent evt)
    {
        Debug.Log("GameOverController: Retry button clicked");

        // Close the Game Over UI
        gameOverContext.Close();

        // Trigger a new run through the game manager
        if (GameManagerComponent.Instance != null)
        {
            GameManagerComponent.Instance.StartNewRun();

            // Transition to the anchor point scene.
            EventBus<OnSceneLoadRequest>.Call(new OnSceneLoadRequest
            {
                scenesToLoad = new List<SceneField> { anchorPointScene },
                scenesToUnload = new List<SceneField> { gameOverScene },
                activeSceneToSet = anchorPointScene,
                showLoadingBar = true,
                unloadAllButPersistent = true
            });
        }
        else
        {
            Debug.LogError("GameOverController: Cannot retry - GameManagerComponent.Instance is null");
        }
    }

    /// <summary>
    /// Handles main menu button click.
    /// </summary>
    private void OnMainMenuClicked(ClickEvent evt)
    {
        Debug.Log("GameOverController: Main menu button clicked");
        
        // Close the Game Over UI
        gameOverContext.Close();

        // Transition to the main scene.
        EventBus<OnSceneLoadRequest>.Call(new OnSceneLoadRequest
        {
            scenesToLoad = new List<SceneField> { mainMenuScene },
            scenesToUnload = new List<SceneField> { gameOverScene },
            activeSceneToSet = mainMenuScene,
            showLoadingBar = true,
            unloadAllButPersistent = true
        });
    }

    private void OnDestroy()
    {
        // Unregister event
        if (gameOverBinding != null)
        {
            EventBus<OnGameOverEvent>.Unregister(gameOverBinding);
        }

        // Unregister button callbacks
        if (retryButton != null)
        {
            retryButton.UnregisterCallback<ClickEvent>(OnRetryClicked);
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.UnregisterCallback<ClickEvent>(OnMainMenuClicked);
        }
    }
}
