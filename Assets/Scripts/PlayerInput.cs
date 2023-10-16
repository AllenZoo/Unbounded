using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> OnMovementInput;

    // Handles all inputs
    private void Handle_Input()
    {
        Handle_Movement_Input();
    }
    
    // Handles AWSD, arrow key movement
    private void Handle_Movement_Input()
    {
        // Handle movement input.
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Fire event to move player.
        Vector2 movementInput = new Vector2(horizontal, vertical);
        OnMovementInput?.Invoke(movementInput);
    }

    private void Update()
    {
        Handle_Input();
    }
}
