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
    public static AttackComponent SpawnAttack(AttackSpawnInfo info, Transform spawnerPos, List<EntityType> targetTypes, GameObject attackObj)
    {
        // TODO: clean up later.
        return null;
        //Attack attack = attackObj.GetComponent<AttackComponent>();
        //Assert.IsNotNull(attack, "To spawn attack obj, it must have an attack component!");

        //if (attack == null)
        //{
        //    Debug.LogError("AttackSpawner: attackObj does not have Attack component!");
        //    return null;
        //}

        //// Offset from attacker. TODO: make this a better calculation.
        //float offset = 0.5f;

        //Vector3 direction = info.mousePosition - spawnerPos.position;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + attack.Data.rotOffset));
        //Vector2 spawnPos = direction.normalized * offset + spawnerPos.transform.position;

        //// Check if attackObj is in pool, use it. else, instantiate new one.
        //// Spawn attack object a certain distance from attacker, rotated towards mouse.
        //GameObject newAttackObj = AttackPool.Instance.GetAttack(attackObj);
        //newAttackObj.transform.position = spawnPos;
        //newAttackObj.transform.rotation = rotation;
        //newAttackObj.SetActive(true);

        //Attack newAttack = newAttackObj.GetComponent<Attack>();
        //newAttack.ResetAttackAfterTime(newAttack.Data.duration);

        //// Set velocity of attack (get from Attack in attackObj)
        //newAttackObj.GetComponent<Rigidbody2D>().velocity = direction.normalized * attack.Data.initialSpeed;

        //// Set valid EntityType targets for attack.
        //newAttack.TargetTypes = targetTypes;

        //// Set sprite of attack.
        ////newAttack.GetComponent<SpriteRenderer>().sprite = attack.Data.attackSprite;

        //return newAttack;
    }

    /// <summary>
    /// Spawns attack object torwards direction from spawnerPos.
    /// </summary>
    /// <param name="direction">direction to spawn the attack torwards</param>
    /// <param name="spawnerPos">the transform to spawn the attack at</param>
    /// <param name="targetTypes">the targets the spawned attack can hit</param>
    /// <param name="attackObj">the actual attack object pfb</param>
    /// <param name="attacker">the attacker. Needed for setting attackObj data.</param>
    /// <param name="atkStat">the atk stat to set on atk obj</param>
    /// <param name="percentageDamageIncrease">the % increase buff to apply to attack</param>
    /// <returns>The newly created attack</returns>
    public static AttackComponent SpawnAttack(Vector3 direction, Transform spawnerPos, List<EntityType> targetTypes, GameObject attackObj, Attacker attacker, float atkStat, double percentageDamageIncrease)
    {
        AttackComponent attackComponent = attackObj.GetComponent<AttackComponent>();

        Assert.IsNotNull(attackComponent, "To spawn attack obj, it must have an attack component!");
        if (attackComponent == null)
        {
            Debug.LogError("AttackSpawner: attackObj does not have Attack component!");
            return null;
        }

        // Dereference a bit to make things less messy.
        // We want to set AttackComponent of spawned object with the data passed in from attacker.
        AttackData attackerAttackData = attacker.AttackData;

        // Offset from attacker. TODO: make this a better calculation.
        float offset = 0.5f;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + attackerAttackData.rotOffset));
        Vector2 spawnPos = direction.normalized * offset + spawnerPos.transform.position;

        // Check if attackObj is in pool, use it. else, instantiate new one.
        // Spawn attack object a certain distance from attacker, rotated towards mouse.
        GameObject newAttackObj = AttackPool.Instance.GetAttack(attackObj);
        newAttackObj.transform.position = spawnPos;
        newAttackObj.transform.rotation = rotation;
        newAttackObj.SetActive(true);

        // Set dynamic attack fields and reset timer.
        var newAttack = newAttackObj.GetComponent<AttackComponent>();
        newAttack.Attack.SetAtkStat(atkStat);
        newAttack.Attack.SetPercentageDamageIncrease(percentageDamageIncrease);

        var duration = attackerAttackData.distance / attackerAttackData.initialSpeed;
        Debug.Log($"Spawning attack with duration: {duration}");
        newAttack.ResetAttackAfterTime(duration);
        newAttack.Attack.SetAtkData(attackerAttackData);

        // Set velocity of attack (get from Attack in attackObj)
        newAttackObj.GetComponent<Rigidbody2D>().velocity = direction.normalized * attackerAttackData.initialSpeed;

        // Set valid EntityType targets for attack.
        newAttack.TargetTypes = targetTypes;

        return newAttack;
    }
}
