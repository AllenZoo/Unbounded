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
            playerInput.enabled = enabled;
        }
    }
}
