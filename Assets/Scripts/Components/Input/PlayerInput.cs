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

    private bool isAttacking;

    protected void Awake()
    {
        base.Awake();

        if (attack == null || attack.action == null)
        {
            Debug.LogError("Attack input action is not assigned!");
            return;
        }

    }

    private void OnEnable()
    {
        attack.action.performed += OnAttackPerformed;
        attack.action.canceled += OnAttackCanceled;

        attack.action.Enable();
    }

    private void OnDisable()
    {
        attack.action.performed -= OnAttackPerformed;
        attack.action.canceled -= OnAttackCanceled;

        attack.action.Disable();
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
        // Handle attack input (left click or just pressed)
        if (isAttacking)
        {
            // Mouse position in world space
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            AttackSpawnInfo info = new AttackSpawnInfo(worldPos);
            base.InvokeAttackInput(KeyCode.Mouse0, info);
        }
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        isAttacking = true;
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        isAttacking = false;
    }

    private void Update()
    {
        Handle_Input();
    }
}
