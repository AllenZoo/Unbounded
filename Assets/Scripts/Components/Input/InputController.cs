using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public event Action<Vector2> OnMovementInput;
    public event Action<KeyCode, AttackSpawnInfo> OnAttackInput;
    public bool inputEnabled = true;

    public void InvokeMovementInput(Vector2 movementInput)
    {
        if (!inputEnabled)
        {
            return;
        }

        OnMovementInput?.Invoke(movementInput);
    }

    public void InvokeAttackInput(KeyCode keyCode, AttackSpawnInfo attackInfo)
    {
        if (!inputEnabled)
        {
            return;
        }

        OnAttackInput?.Invoke(keyCode, attackInfo);
    }

}
