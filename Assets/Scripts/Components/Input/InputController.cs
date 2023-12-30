using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public event Action<Vector2> OnMovementInput;
    public event Action<KeyCode, AttackSpawnInfo> OnAttackInput;

    public void InvokeMovementInput(Vector2 movementInput)
    {
        OnMovementInput?.Invoke(movementInput);
    }

    public void InvokeAttackInput(KeyCode keyCode, AttackSpawnInfo attackInfo)
    {
        OnAttackInput?.Invoke(keyCode, attackInfo);
    }

}
