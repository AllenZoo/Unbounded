using UnityEngine;

public class LinearMovement : IAttackMovement
{
    private Vector3 velocity;
    public void Init(AttackComponent ac, AttackData data, AttackContext context, AttackModificationContext amc)
    {
        // Direct Vector from attacker to target
        Vector2 baseDir = (context.AttackSpawnInfo.targetPosition - context.AttackerTransform.position).normalized;


        // Check if we need to override the attack direction
        if (amc.AttackDirection != Vector3.zero)
        {
            baseDir = amc.AttackDirection.normalized;
        }

        // Angle to apply to the base direction
        float angle = amc.AngleOffset; // + data.SpriteRotOffset;

        // Calculate initial position and rotation
        Vector2 dir = Quaternion.Euler(0f, 0f, angle) * baseDir;
        ac.transform.position = (Vector3)(dir * 0.5f) + context.AttackerTransform.position;

        // Rotate Object to look at target
        float zAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        ac.transform.rotation = Quaternion.Euler(0f, 0f, zAngle );

        // Set Velocity for movement
        velocity = dir * data.InitialSpeed;

        // Visual
        float visualAngle =
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg
            + data.SpriteRotOffset;

        ac.transform.rotation =
            Quaternion.Euler(0f, 0f, visualAngle);
    }
    public void UpdateMovement(AttackComponent ac, Rigidbody2D rb)
    {
        // Apply Velocity
        rb.linearVelocity = velocity;

        // Trigger Launch
        ac.TriggerAttackLaunch();
    }
}
