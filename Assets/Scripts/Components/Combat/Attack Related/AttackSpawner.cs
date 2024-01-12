using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Class that spawns attacks.
public class AttackSpawner
{
    /// <summary>
    /// Spawns attack object from attackObj at spawnerPos, towards mousePosition (or attack input position for monsters).
    /// </summary>
    /// <param name="info"></param>
    /// <param name="spawnerPos"></param>
    /// <param name="targetTypes"></param>
    /// <param name="attackObj"></param>
    /// <returns>The newly created Attack.</returns>
    public static Attack SpawnAttack(AttackSpawnInfo info, Transform spawnerPos, List<EntityType> targetTypes, GameObject attackObj)
    {
        Attack attack = attackObj.GetComponent<Attack>();
        Assert.IsNotNull(attack, "To spawn attack obj, it must have an attack component!");

        if (attack == null)
        {
            Debug.LogError("AttackSpawner: attackObj does not have Attack component!");
            return null;
        }

        // Offset from attacker. TODO: make this a better calculation.
        float offset = 0.5f;

        Vector3 direction = info.mousePosition - spawnerPos.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + attack.Data.rotOffset));
        Vector2 spawnPos = direction.normalized * offset + spawnerPos.transform.position;

        // Check if attackObj is in pool, use it. else, instantiate new one.
        // Spawn attack object a certain distance from attacker, rotated towards mouse.
        GameObject newAttackObj = AttackPool.Instance.GetAttack(attackObj);
        newAttackObj.transform.position = spawnPos;
        newAttackObj.transform.rotation = rotation;
        newAttackObj.SetActive(true);

        Attack newAttack = newAttackObj.GetComponent<Attack>();
        newAttack.ResetAttackAfterTime(newAttack.Data.duration);

        // Set velocity of attack (get from Attack in attackObj)
        newAttackObj.GetComponent<Rigidbody2D>().velocity = direction.normalized * attack.Data.initialSpeed;

        // Set valid EntityType targets for attack.
        newAttack.TargetTypes = targetTypes;

        // Set sprite of attack.
        //newAttack.GetComponent<SpriteRenderer>().sprite = attack.Data.attackSprite;

        return newAttack;
    }

    /// <summary>
    /// Spawns attack object torwards direction from spawnerPos.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="spawnerPos"></param>
    /// <param name="targetTypes"></param>
    /// <param name="attackObj"></param>
    /// <returns>The newly created Attack.</returns>
    public static Attack SpawnAttack(Vector3 direction, Transform spawnerPos, List<EntityType> targetTypes, GameObject attackObj)
    {
        Attack attack = attackObj.GetComponent<Attack>();
        Assert.IsNotNull(attack, "To spawn attack obj, it must have an attack component!");

        if (attack == null)
        {
            Debug.LogError("AttackSpawner: attackObj does not have Attack component!");
            return null;
        }

        // Offset from attacker. TODO: make this a better calculation.
        float offset = 0.5f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + attack.Data.rotOffset));
        Vector2 spawnPos = direction.normalized * offset + spawnerPos.transform.position;

        // Check if attackObj is in pool, use it. else, instantiate new one.
        // Spawn attack object a certain distance from attacker, rotated towards mouse.
        GameObject newAttackObj = AttackPool.Instance.GetAttack(attackObj);
        newAttackObj.transform.position = spawnPos;
        newAttackObj.transform.rotation = rotation;
        newAttackObj.SetActive(true);

        Attack newAttack = newAttackObj.GetComponent<Attack>();
        newAttack.ResetAttackAfterTime(newAttack.Data.duration);

        // Set velocity of attack (get from Attack in attackObj)
        newAttackObj.GetComponent<Rigidbody2D>().velocity = direction.normalized * attack.Data.initialSpeed;

        // Set valid EntityType targets for attack.
        newAttack.TargetTypes = targetTypes;

        // Set sprite of attack.
        //newAttack.GetComponent<SpriteRenderer>().sprite = attack.Data.attackSprite;

        return newAttack;
    }
}
