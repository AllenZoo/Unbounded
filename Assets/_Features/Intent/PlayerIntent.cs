using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(StateComponent))]
public class PlayerIntent : IntentController
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
        move.action.Enable();
    }

    private void OnDisable()
    {
        attack.action.performed -= OnAttackPerformed;
        attack.action.canceled -= OnAttackCanceled;

        attack.action.Disable();
        move.action.Disable();
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
        //if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        //    return;

        if (IsPointerOverBlockingUI())
            return;

        isAttacking = true;
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        isAttacking = false;
    }

    // TODO: look over and polish
    private bool IsPointerOverBlockingUI()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // ---------- UGUI CHECK ----------
        if (EventSystem.current != null)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.GetComponentInParent<BlockGameplayInput>() != null)
                    return true;
            }
        }

        // ---------- UI TOOLKIT CHECK ----------
        //foreach (var document in FindObjectsOfType<UIDocument>())
        //{
        //    if (document.rootVisualElement == null)
        //        continue;

        //    var picked = document.rootVisualElement.panel.Pick(mousePosition);

        //    if (picked != null && picked.ClassListContains("block-gameplay"))
        //        return true;
        //}

        return false;
    }


    private void Update()
    {
        Handle_Input();
    }
}
