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

    private void Awake()
    {
        EventBinding<OnPauseChangeRequest> pauseChangeRequestBinding = new EventBinding<OnPauseChangeRequest>(OnPauseChangeRequestEvent);
        EventBus<OnPauseChangeRequest>.Register(pauseChangeRequestBinding);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    private void OnPauseChangeRequestEvent(OnPauseChangeRequest pauseRequest) {
        Debug.Log($"Received pause request. Should pause: {pauseRequest.shouldPause}");

        if (pauseRequest.shouldPause) Pause();
        else Resume();
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
}
