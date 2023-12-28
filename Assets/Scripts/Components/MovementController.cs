using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MotionComponent))]
[RequireComponent(typeof(StatComponent))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
    private MotionComponent motion;
    private StatComponent stat;
    private Rigidbody2D rb;

    private void Awake()
    {
        motion = GetComponent<MotionComponent>();
        stat = GetComponent<StatComponent>();
        rb = GetComponent<Rigidbody2D>();
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

    private void FixedUpdate()
    {
        HandleMovement();
    }
}
