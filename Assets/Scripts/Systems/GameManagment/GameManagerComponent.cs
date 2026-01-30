using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.Rendering.DebugManager;

public class GameManagerComponent : Singleton<GameManagerComponent>
{
    public GameState State { get; private set; }
    public RoomState RoomState { get; private set; }

    public int roundNumber = 1; // Round number, (start at 1)

    protected override void Awake()
    {
        base.Awake();
        ChangeState(GameState.MainMenu);

        EventBinding<OnSceneLoadRequestFinish> osclrfBinding = new EventBinding<OnSceneLoadRequestFinish>(Handle_OnSceneLoadRequestFinish);
        EventBus<OnSceneLoadRequestFinish>.Register(osclrfBinding);
    }

    protected void Start()
    {
        Handle_OnSceneLoadRequestFinish();
    }

    public void StartNewRun()
    {
        //CurrentRun = new RunData
        //{
        //    Mode = mode,
        //    CurrentFloor = 0,
        //    PlayerHP = 100
        //};

        ChangeState(GameState.WeaponTrial);
    }

    public void OnPlayerDeath()
    {
        ChangeState(GameState.RunEnd);
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
