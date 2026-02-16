using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateComponent))]
public class PlayerInput : InputController
{
    public bool InputEnabled { get; set; } = true;

    [Required, SerializeField] private InputActionReference move;
    [Required, SerializeField] private InputActionReference attack;

    protected void Awake()
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
        //float horizontal = Input.GetAxis("Horizontal");
        //float vertical = Input.GetAxis("Vertical");

        if (move == null || move.action == null)
        {
            Debug.LogError("Move input action is not assigned!");
            return;
        }

        // Send movement input event
        Vector2 movementInput = move.action.ReadValue<Vector2>();
        base.InvokeMovementInput(movementInput);
    }

    private void Handle_Attack_Input()
    {
        if (attack == null || attack.action == null)
        {
            Debug.LogError("Attack input action is not assigned!");
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Handle attack input (left click or just pressed)
        if (attack.action.IsPressed())
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
