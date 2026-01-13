using UnityEngine;

public class LinearMovement : IAttackMovement
{
    private Vector3 velocity;
    public void Init(AttackComponent ac, AttackData data, AttackContext context)
    {
        Vector3 dir = context.SpawnInfo.mousePosition - context.AttackerTransform.position;

        float offset = 0.5f;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + data.RotOffset));
        Vector2 spawnPos = dir.normalized * offset + context.AttackerTransform.position;
        ac.transform.position = spawnPos;

        velocity = dir.normalized * data.InitialSpeed;
    }
    public void UpdateMovement(AttackComponent ac, Rigidbody2D rb)
    {
        rb.linearVelocity = velocity;
    }
}
