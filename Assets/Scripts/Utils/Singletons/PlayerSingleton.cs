using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : Singleton<PlayerSingleton>
{
    public StatComponent GetPlayerStatComponent()
    {
        return GetComponentInChildren<StatComponent>();
    }

    public void TogglePlayerInput(bool enabled)
    {
        var playerInput = GetComponentInChildren<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.InputEnabled = enabled;
        }
    }

    public void ResetPlayer()
    {
        // Ensure player is active
        gameObject.SetActive(true);

        // Reset state to IDLE
        StateComponent stateComp = GetComponent<StateComponent>();
        if (stateComp != null) stateComp.ResetState();

        // Reset health and other combat flags via Respawn event
        LocalEventHandler leh = GetComponent<LocalEventHandler>();
        if (leh != null) leh.Call(new OnRespawnEvent());

        // Ensure input is enabled
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.InputEnabled = true;
        }
    }
}
