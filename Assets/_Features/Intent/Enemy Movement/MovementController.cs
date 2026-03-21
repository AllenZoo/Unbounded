using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

[RequireComponent(typeof(MotionComponent))]
public class MovementController : MonoBehaviour
{
    // For smoothing movement.
    [Tooltip("High acceleration means faster changes in velocity, and thus more responsive movement.")]
    [SerializeField] private float acceleration = 50f;

    [Tooltip("High deceleration means faster changes in velocity, and thus more responsive stopping.")]
    [SerializeField] private float deceleration = 50f;

    [SerializeField, ReadOnly] private const float SPEED_SCALE = 2/5f;


    [SerializeField] private LocalEventHandler leh;
    [SerializeField] private bool movementEnabled = true;
    [SerializeField] private StatComponent stat;
    [SerializeField] private Rigidbody2D rb;

    private MotionComponent motion;
    
    private void Awake()
    {
        motion = GetComponent<MotionComponent>();

        if (stat == null)
        {
            stat = GetComponent<StatComponent>();
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        

        Assert.IsNotNull(motion, "Class of type MotionComponent must exist on obj with MovementController");
        Assert.IsNotNull(stat, "Class of type StatComponent must exist on obj with MovementController");
        Assert.IsNotNull(rb, "Rigidbody2D must exist on obj with MovementController");

        leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);
        Assert.IsNotNull(leh, "LocalEventHandler must exist in parent of obj with MovementController");
    }

    private void Start()
    {
        LocalEventBinding<OnStateChangeEvent> stateChangeBinding = new LocalEventBinding<OnStateChangeEvent>(HandleOnStateChange);
        leh.Register(stateChangeBinding);
    }

    private void FixedUpdate()
    {
        if (movementEnabled)
        {
            HandleMovement();
        }
    }

    private void HandleOnStateChange(OnStateChangeEvent e)
    {
        switch (e.newState)
        {
            case State.STUNNED:
                SetMovementEnabled(false);
                break;
            case State.DEAD:
                SetMovementEnabled(false);
                ResetMovementVelocity();
                break;
            default:
                SetMovementEnabled(true);
                break;
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
    }

    // Resets the rb velocity to zero.
    public void ResetMovementVelocity()
    {
        rb.linearVelocity = Vector3.zero;
    }

    private float lastLoggedSpeed = -1f;

    // Calculates velocity and moves gameobject appropriately
    private void HandleMovement()
    {
        float currentSpeed = stat.StatContainer.Speed;
        if (Mathf.Abs(currentSpeed - lastLoggedSpeed) > 0.01f)
        {
            // Only log if it's a significant change
            Debug.Log($"[{gameObject.name}] Current Speed: {currentSpeed} (Target Velocity Magnitude: {currentSpeed * SPEED_SCALE})");
            lastLoggedSpeed = currentSpeed;
        }

        // Normalize diagonal input
        Vector2 processedDir = motion.Dir.sqrMagnitude > 1
            ? motion.Dir.normalized
            : motion.Dir;

        // Target velocity (no deltaTime scaling here)
        Vector2 targetVelocity = processedDir * currentSpeed * SPEED_SCALE;

        // Choose accel or decel
        float rate = processedDir == Vector2.zero ? deceleration : acceleration;

        // Smooth toward target velocity
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            targetVelocity,
            rate * Time.fixedDeltaTime
        );

    }

}
