using System;
using System.Collections.Generic;
using UnityEngine;

// Class that spawns attacks.
public class AttackSpawner
{
    public static List<AttackComponent> SpawnGroup(
        int count,
        Func<int, IAttackMovement> movementFactory,
        AttackData data,
        AttackContext context,
        AttackModificationContext amc,
        IAttack logic,
        Func<int, Quaternion> rotationFactory = null)
    {
        List<AttackComponent> spawned = new(count);

        for (int i = 0; i < count; i++)
        {
            var movement = movementFactory(i);

            var attack = Spawn(
                data,
                context,
                amc,
                logic,
                movement
            );

            if (attack != null)
            {
                if (rotationFactory != null)
                {
                    attack.transform.rotation = rotationFactory(i);
                }

                spawned.Add(attack);
            }
        }

        return spawned;
    }


    public static AttackComponent Spawn(
        AttackData data,
        AttackContext context,
        AttackModificationContext amc,
        IAttack logic,
        IAttackMovement movement)
    {
        GameObject go = AttackPool.Instance.GetAttack(data.AttackPfb);
        go.SetActive(true);

        AttackComponent attackComponent = go.GetComponent<AttackComponent>();

        // Initialize movement
        if (movement == null)
        {
            Debug.LogError("AttackSpawner: Spawn: movement is null!");
            return null;
        }
        movement.Init(attackComponent, data, context, amc);

        // Inject behaviors
        attackComponent.Initialize(data, context, logic, movement);

        // Apply Attack Modifications
        if (attackComponent.TryGetComponent<CircleScaler>(out var scaler))
        {
            scaler.SetCircleRadius(amc.ObjectScale);
        }

        if (amc.AttackDuration > 0)
        {
            attackComponent.ResetAttackAfterTime(amc.AttackDuration);
        }


        return attackComponent;
    }
}
