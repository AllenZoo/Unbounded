using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles pausing the game, while listening for OnPauseStateChange requests.
/// </summary>
public class PauseManager : MonoBehaviour
{
    public static bool IsPaused { get; private set; } = false;

    [SerializeField] private PlayerInput playerInput; // Coupled with player input to allow disabling it. Should be fine to couple this logic since they will both always exist in persistent gameplay (or should, will add guards).

    private void Awake()
    {
        Debug.Assert(playerInput != null);
        EventBinding<OnPauseChangeRequest> pauseChangeRequestBinding = new EventBinding<OnPauseChangeRequest>(OnPauseChangeRequestEvent);
        EventBus<OnPauseChangeRequest>.Register(pauseChangeRequestBinding);
    }

    private void OnPauseChangeRequestEvent(OnPauseChangeRequest pauseRequest) {
        Debug.Log($"Received pause request. Should pause: {pauseRequest.shouldPause}");

        if (pauseRequest.shouldPause)
        {
            //Pause();
            PausePlayerInput();
        }
        else {
            //Resume();
            ResumePlayerInput();
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
