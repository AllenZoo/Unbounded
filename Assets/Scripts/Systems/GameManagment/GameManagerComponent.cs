using Sirenix.OdinInspector;
using UnityEngine;
using static UnityEngine.Rendering.DebugManager;

public class GameManagerComponent : Singleton<GameManagerComponent>
{
    public GameState State { get; private set; }
    //public RunData CurrentRun { get; private set; }

    public void StartNewRun(RunMode mode)
    {
        //CurrentRun = new RunData
        //{
        //    Mode = mode,
        //    CurrentFloor = 0,
        //    PlayerHP = 100
        //};

        ChangeState(GameState.WeaponTrial);
    }

    //public void SelectWeapon(WeaponData weapon)
    //{
    //    CurrentRun.SelectedWeapon = weapon;
    //    LoadFirstFloor();
    //}

    public void OnPlayerDeath()
    {
        ChangeState(GameState.RunEnd);
    }

    public void OnFinalBossDefeated()
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

}

public enum GameState
{
    MainMenu,
    WeaponTrial,
    InRun,
    BossFight,
    RunEnd
}

public enum RunMode
{
    Tutorial,
    Normal
}
