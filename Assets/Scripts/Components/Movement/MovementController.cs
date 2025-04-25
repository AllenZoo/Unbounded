using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(MotionComponent))]
public class MovementController : MonoBehaviour
{
    [SerializeField] private LocalEventHandler localEventHandler;
    [SerializeField] private bool movementEnabled = true;
    [SerializeField] private StatComponent stat;
    [SerializeField] private Rigidbody2D rb;


    /* Unmerged change from project 'Assembly-CSharp.Player'
    Before:
        private MotionComponent motion;

        private void Awake()
    After:
        private MotionComponent motion;

        private void Awake()
    */
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

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                                                  "] with root object [" + gameObject.transform.root.name + "] for MovementController.cs");
            }
        }
    }

    private void Start()
    {
        LocalEventBinding<OnStateChangeEvent> stateChangeBinding = new LocalEventBinding<OnStateChangeEvent>(HandleOnStateChange);
        localEventHandler.Register(stateChangeBinding);
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
        rb.velocity = Vector3.zero;
    }

    // Calculates velocity and moves gameobject appropriately
    private void HandleMovement()
    {
        float scale = 100;

        // 0. Normalize motion.dir vector if it exceeeds magnitude of 1 (diagonal movement)
        Vector2 processed_dir = motion.dir.sqrMagnitude > 1 ? motion.dir.normalized : motion.dir;

        // 1. Velocity Calculation
        Vector2 velocity = processed_dir * stat.speed * Time.fixedDeltaTime * scale;

        // 2. Apply translation
        rb.velocity = velocity;
    }
}
