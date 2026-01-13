using UnityEngine;

public class MeteorFallMovement : IAttackMovement
{
    private Vector3 targetPos;
    public void Init(AttackComponent ac, AttackData data, AttackContext context)
    {
        targetPos = context.SpawnInfo.mousePosition;

        // Top right of camera
        Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 1.2f, 0));
        ac.transform.position = spawnPos;

    }
    public void UpdateMovement(AttackComponent ac, Rigidbody2D rb)
    {
        ac.transform.position = targetPos;
    }
}
