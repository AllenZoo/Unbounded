using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugManager;

public class GameManagerComponent : Singleton<GameManagerComponent>, IDataPersistence
{
    public GameState State { get; private set; }
    public RoomState RoomState { get; private set; }

    public int roundNumber = 1; // Round number, (start at 1)

    EventBinding<OnPlayerDeathEvent> playerDeathBinding;

    protected override void Awake()
    {
        base.Awake();
        ChangeState(GameState.MainMenu);

        EventBinding<OnSceneLoadRequestFinish> osclrfBinding = new EventBinding<OnSceneLoadRequestFinish>(Handle_OnSceneLoadRequestFinish);
        EventBus<OnSceneLoadRequestFinish>.Register(osclrfBinding);

        playerDeathBinding = new EventBinding<OnPlayerDeathEvent>(OnPlayerDeath);
    }

    protected void Start()
    {
        Handle_OnSceneLoadRequestFinish();
        playerDeathBinding = new EventBinding<OnPlayerDeathEvent>(OnPlayerDeath);
    }

    private void OnEnable()
    {
        if (playerDeathBinding != null) EventBus<OnPlayerDeathEvent>.Register(playerDeathBinding);
    }
    private void OnDisable()
    {
        if (playerDeathBinding != null) EventBus<OnPlayerDeathEvent>.Unregister(playerDeathBinding);
    }

    public void StartNewRun()
    {
        DataPersistenceHandler.Instance.NewGame();

        // Reset Player State and Input
        if (PlayerSingleton.Instance != null)
        {
            PlayerSingleton.Instance.ResetPlayer();
        }

        // Initialize run tracking
        if (RunTracker.Instance != null)
        {
            RunTracker.Instance.StartNewRun();
        }

        // Initialize run history tracking
        if (RunHistoryManager.Instance != null)
        {
            RunHistoryManager.Instance.StartNewRun();
        }

        ChangeState(GameState.WeaponTrial);
        roundNumber = 1;
    }

    public void OnPlayerDeath()
    {
        // End run tracking and calculate final score
        if (RunTracker.Instance != null)
        {
            int finalScore = RunTracker.Instance.EndRun();
            Debug.Log($"Run ended with final score: {finalScore}");
            
            // Create score summary data and trigger game over event
            ScoreSummaryData scoreSummary = ScoreSummaryData.FromRunData(RunTracker.Instance.CurrentRun);
            EventBus<OnGameOverEvent>.Call(new OnGameOverEvent { scoreSummary = scoreSummary });

            // Reset Weapon
            EventBus<OnResetWeaponRequest>.Call(new OnResetWeaponRequest());

            // Reset Game Data to a "New Run" state (preserving high scores)
            DataPersistenceHandler.Instance.ResetToNewState();
        }

        ChangeState(GameState.RunEnd);
    }

    public void LoadData(GameData data)
    {
        roundNumber = data.roundNumber;
    }

    public void SaveData(GameData data)
    {
        data.roundNumber = roundNumber;
    }

    public void ResetData()
    {
        roundNumber = 1;
    }

    private void ChangeState(GameState newState)
    {
        State = newState;

        switch (State)
        {
            case GameState.MainMenu:
                //LoadScene("MainMenu");
                break;

            case GameState.WeaponTrial:
                //LoadScene("WeaponTrial");
                break;

            case GameState.InRun:
                //LoadScene("GameplayArena");
                break;

            case GameState.BossFight:
                //LoadScene("BossArena");
                break;

            case GameState.RunEnd:
                //LoadScene("RunEnd");
                break;
        }
    }

    private void ChangeState(RoomState newRoomState)
    {
        RoomState = newRoomState;
        switch (RoomState)
        {
            case RoomState.Menu:
                break;
            case RoomState.WeaponTrialRoom:
                break;
            case RoomState.HomeRoom:
                HandleOnHomeRoomState();
                break;
            case RoomState.BossRoom:
                HandleOnBossRoomState();
                break;
        }
    }

    private void Handle_OnSceneLoadRequestFinish()
    {
        // Check room we are in. If in HomeRoom, get a random boss selection.
        if (RoomStateCategorizer.Instance == null)
        {
            Debug.LogError("RoomStateCategorizer Instance is null!");
            return;
        }

        RoomState roomState = RoomStateCategorizer.Instance.GetRoomStateForActiveScene();
        ChangeState(roomState);
    }

    /// <summary>
    /// Helper called whenever we change RoomState to HoomRoom.
    /// </summary>
    private void HandleOnHomeRoomState()
    {
        ManageBossSelection();
    }

    private void HandleOnBossRoomState()
    {
        // Increment Round
        roundNumber++;
    }

    private void ManageBossSelection()
    {
        var entry = BossSelectorComponent.Instance.GetNextBoss();
        BossPortalComponent.Instance.SetBossTeleportation(entry.bossScene);
    }



    // Context menu for testing in editor
    [Button, GUIColor(1, 0, 1)]
    private void HandleOnRoomState()
    {
        HandleOnHomeRoomState();
    }
}

public enum GameState
{
    MainMenu,
    WeaponTrial,
    InRun,
    InHomeRoom,
    BossFight,
    RunEnd
}

public enum RoomState
{
    Null,
    Menu,
    WeaponTrialRoom,
    HomeRoom, // Anchorpoint
    BossRoom,
}
