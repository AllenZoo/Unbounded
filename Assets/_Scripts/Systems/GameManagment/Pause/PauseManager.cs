using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles pausing the game. Uses a token system. Systems that requests a pause
/// will receive a token, and the game will remain paused until all tokens are released. This allows multiple systems to request a pause without interfering with each other.
/// For example, if the player opens the inventory (which requests a pause) and then opens the map (which also requests a pause), 
/// the game will remain paused until both the inventory and map are closed (both tokens released).
/// </summary>
public class PauseManager : Singleton<PauseManager>
{
    public static bool IsPaused { get; private set; } = false;

    [SerializeField] private PlayerInput playerInput; // Coupled with player input to allow disabling it. Should be fine to couple this logic since they will both always exist in persistent gameplay (or should, will add guards).

    private HashSet<Guid> activeTokens = new();

    protected override void Awake()
    {
        base.Awake();
        Debug.Assert(playerInput != null);
    }

    public PauseToken RequestPause()
    {
        Guid guid = Guid.NewGuid();
        PauseToken token = new PauseToken
        {
            guid = guid,
            manager = this,
            isReleased = false
        };
        activeTokens.Add(guid);
        PausePlayerInput();

        return token;
    }

    public void UpdatePauseState()
    {
        if (activeTokens.Count > 0)
        {
            PausePlayerInput();
        }
        else
        {
            ResumePlayerInput();
        }
    }

    public void ReleasePause(Guid tokenGuid)
    {
       if (activeTokens.Remove(tokenGuid))
        {
            UpdatePauseState();
        }
    }


    public void Pause()
    {
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }

    public void PausePlayerInput()
    {
        if (playerInput == null)
        {
            Debug.LogError("Player input is null! Not assigned to pause manager");
        }
        playerInput.InputEnabled = false;
    }

    public void ResumePlayerInput()
    {
        if (playerInput == null)
        {
            Debug.LogError("Player input is null! Not assigned to pause manager");
        }
        playerInput.InputEnabled = true;
    }
}
