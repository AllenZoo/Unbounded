using UnityEngine;
using DG.Tweening;

public class MeteorFallMovement : IAttackMovement
{
    private Vector3 targetPos;
    private float timeToTarget;

    public void Init(AttackComponent attackComponent, AttackData data, AttackContext context)
    {
        // 1. Calculate Spawn Point (Sky)
        Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 1.2f, 0));
        spawnPos.z = 0f;
        attackComponent.transform.position = spawnPos;


        targetPos = context.SpawnInfo.mousePosition;

        // Use speed to determine time, or just a fixed duration
        timeToTarget = 1.0f;

        // 2. Start the fall
        attackComponent.transform.DOMove(targetPos, timeToTarget)
            .OnStart(() => attackComponent.TriggerAttackLaunch())
            .SetEase(Ease.InQuad)  
            .OnComplete(() => attackComponent.TriggerAttackLand()); // We add TriggerLand to AttackComponent
    }

    public void UpdateMovement(AttackComponent ac, Rigidbody2D rb)
    {
        // DO nothing here because DOTween is handling it
    }
}