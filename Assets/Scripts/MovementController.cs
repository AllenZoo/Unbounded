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

        // 1. Velocity Calculation
        Vector2 velocity = motion.dir * stat.speed * Time.fixedDeltaTime * scale;

        // 2. Apply translation
        rb.velocity = velocity;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }
}
