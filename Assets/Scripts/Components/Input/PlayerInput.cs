using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateComponent))]
public class PlayerInput : InputController
{
    public bool InputEnabled { get; set; } = true;

    private void Awake()
    {
        base.Awake();
    }

    // Handles all inputs
    private void Handle_Input()
    {
        if (!InputEnabled) {
            Debug.Log("Player input is currently disabled!");
            base.InvokeMovementInput(Vector2.zero); // To set the movement to nothing, incase previous input not reset.
            return;
        }
        Handle_Movement_Input();
        Handle_Attack_Input();
    }
    
    // Handles AWSD, arrow key movement
    private void Handle_Movement_Input()
    {
        // Handle movement input.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Send movement input event
        Vector2 movementInput = new Vector2(horizontal, vertical);
        base.InvokeMovementInput(movementInput);
    }

    private void Handle_Attack_Input()
    {
        // Disable attack input when UI is active
        if (UIStateManager.Instance != null && UIStateManager.Instance.IsUIActive)
        {
            return;
        }

        // Handle attack input (left click or just pressed)
        if (Input.GetMouseButton(0))
        {
            // Mouse position in world space
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            AttackSpawnInfo info = new AttackSpawnInfo(mousePos);
            base.InvokeAttackInput(KeyCode.Mouse0, info);
        }
    }

    private void Update()
    {
        Handle_Input();
    }
}
