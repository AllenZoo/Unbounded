using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StateComponent))]
public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> OnMovementInput;
    private StateComponent state;

    private void Awake()
    {
        state = GetComponent<StateComponent>();
    }

    // Handles all inputs
    private void Handle_Input()
    {
        Handle_Movement_Input();
    }
    
    // Handles AWSD, arrow key movement
    private void Handle_Movement_Input()
    {
        // Handle movement input.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Send movement input event
        Vector2 movementInput = new Vector2(horizontal, vertical);
        OnMovementInput?.Invoke(movementInput);

        // Set state to WALKING (handle state in helper later)
        if (horizontal != 0 || vertical != 0)
        {
            state.SetState(State.WALKING);
        } else
        {
            state.SetState(State.IDLE);
        }
    }

    private void Update()
    {
        Handle_Input();
    }
}
