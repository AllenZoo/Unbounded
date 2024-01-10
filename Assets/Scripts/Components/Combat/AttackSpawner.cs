using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that spawns attacks.
public class AttackSpawner
{
    /// <summary>
    /// Spawns attack object from attackObj at spawnerPos, towards mousePosition (or attack input position for monsters).
    /// Attack reference should be the same one as the attackObj.
    /// </summary>
    /// <param name="info"></param>
    /// <param name="spawnerPos"></param>
    /// <param name="targetTypes"></param>
    /// <param name="attackObj"></param>
    /// <param name="attack"></param>
    public static void SpawnAttack(AttackSpawnInfo info, Transform spawnerPos, List<EntityType> targetTypes, GameObject attackObj, Attack attack)
    {
        // Offset from attacker. TODO: make this a better calculation.
        float offset = 1f;

        Vector3 direction = info.mousePosition - spawnerPos.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + attack.RotOffset));
        Vector2 spawnPos = direction.normalized * offset + spawnerPos.transform.position;

        // Check if attackObj is in pool, use it. else, instantiate new one.
        // Spawn attack object a certain distance from attacker, rotated towards mouse.
        GameObject newAttackObj = AttackPool.Instance.GetAttack(attackObj);
        newAttackObj.transform.position = spawnPos;
        newAttackObj.transform.rotation = rotation;
        newAttackObj.SetActive(true);

        Attack newAttack = newAttackObj.GetComponent<Attack>();
        newAttack.ResetAttackAfterTime(newAttack.Duration);

        // Set velocity of attack (get from Attack in attackObj)
        newAttackObj.GetComponent<Rigidbody2D>().velocity = direction.normalized * attack.InitialSpeed;

        // Set valid EntityType targets for attack.
        newAttack.TargetTypes = targetTypes;
    }
}
