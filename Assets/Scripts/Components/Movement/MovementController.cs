using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

[RequireComponent(typeof(MotionComponent))]
public class MovementController : MonoBehaviour
{
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
    }

    private void FixedUpdate()
    {
        if (movementEnabled)
        {
            HandleMovement();
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
