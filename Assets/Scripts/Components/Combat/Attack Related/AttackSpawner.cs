using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Class that spawns attacks.
public class AttackSpawner
{

    /// <summary>
    /// Spawns attack from top right to target point, like a meteor
    /// </summary>
    /// <returns></returns>
    public static AttackComponent SpawnMeteorAttack(Vector3 targetPos, float timeToTarget, GameObject attackPfb, float meteorRadius, List<EntityType> targetTypes)
    {
        // 1. Choose a spawn point above and to the right of the target (or camera)
        Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(1.2f, 1.2f, 0));
        spawnPos.z = 0f;

        // 2. Spawn the meteor prefab
        GameObject newAttackObj = AttackPool.Instance.GetAttack(attackPfb);
        newAttackObj.transform.position = spawnPos;
        //newAttackObj.transform.rotation = rotation; // TODO: impelemnt rotation changes so that attack faces proper dir. Get vector of spawn to target.
        newAttackObj.SetActive(true);


        // 3. Scale attack size based on radius provided
        newAttackObj.GetComponent<CircleScaler>()?.SetCircleRadius(meteorRadius);
        if (newAttackObj.GetComponent<CircleScaler>() == null)
        {
            Debug.LogError("AttackSpawner: SpawnMeteorAttack: attackPfb does not have CircleScaler component!");
        }

        AttackComponent attack = newAttackObj.GetComponent<AttackComponent>();

        // Set duration until attack disappears/resets. (reset = set inactive)
        var duration = timeToTarget + 1f; // TODO: modify this for object reset duration (e.g. timer for attack to despawn)
        attack.ResetAttackAfterTime(duration);

        // 3. Start movement toward the target using DOTween
        newAttackObj.transform.DOMove(targetPos, timeToTarget)
            .SetEase(Ease.Linear)
            .OnStart(() => { attack.TriggerAttackLaunch(); })
            .OnComplete(() =>
            {

                attack.TriggerAttackLand();
                //attack.OnImpact();   // Run damage/explosion logic (TODO: add some logic for this)
            });

        // 4. Set valid EntityType targets for attack.
        // TODO: this will be set indirectly via passing through attack context.
        //attack.TargetTypes = targetTypes;

        return attack;
    }


    /// <summary>
    /// Spawns attack object torwards direction from spawnerPos. Instantiates attack in a created AttackPool.
    /// </summary>
    /// <param name="direction">direction to spawn the attack torwards</param>
    /// <param name="spawnerPos">the transform to spawn the attack at</param>
    /// <param name="targetTypes">the targets the spawned attack can hit</param>
    /// <param name="attackObj">the actual attack object pfb</param>
    /// <param name="attacker">the attacker. Needed for setting attackObj data.</param>
    /// <param name="atkStat">the atk stat to set on atk obj</param>
    /// <param name="percentageDamageIncrease">the % increase buff to apply to attack</param>
    /// <returns>The newly created attack</returns>
    public static AttackComponent SpawnAttackInPool(Vector3 direction, Transform spawnerPos, List<EntityType> targetTypes, GameObject attackObj, IAttacker attacker, float atkStat, double percentageDamageIncrease)
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
        Quaternion rotation = Quaternion.Euler(new Vector3(0f, 0f, angle + attackerAttackData.RotOffset));
        Vector2 spawnPos = direction.normalized * offset + spawnerPos.transform.position;

        // Check if attackObj is in pool, use it. else, instantiate new one.
        // Spawn attack object a certain distance from attacker, rotated towards mouse.
        GameObject newAttackObj = AttackPool.Instance.GetAttack(attackObj);
        newAttackObj.transform.position = spawnPos;
        newAttackObj.transform.rotation = rotation;
        newAttackObj.SetActive(true);

        // Set dynamic attack fields
        var newAttack = newAttackObj.GetComponent<AttackComponent>();
        newAttack.Attack.SetModifiers(atkStat, percentageDamageIncrease);
        newAttack.Attack.AttackData = attackerAttackData;

        // Set duration until attack disappears/resets. (reset = set inactive)
        var duration = attackerAttackData.Distance / attackerAttackData.InitialSpeed;
        newAttack.ResetAttackAfterTime(duration);
        
        // Set velocity of attack (get from Attack in attackObj)
        newAttackObj.GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * attackerAttackData.InitialSpeed;

        // Set valid EntityType targets for attack.
        // TODO: this will be set indirectly via passing through attack context.
        //newAttack.TargetTypes = targetTypes;

        return newAttack;
    }


    /// <summary>
    /// Function that setsup created attacks by assigning the attack component the relevant target type.
    /// </summary>
    /// <param name="attackObj"></param>
    /// <param name="targetTypes"></param>
    public static void SetUpAttack(GameObject attackObj, List<EntityType> targetTypes)
    {
        // Check for an attack component on object
        AttackComponent attackComponent = attackObj.GetComponent<AttackComponent>();
        Assert.IsNotNull(attackComponent, "Attack Object must have an attack component!");
        if (attackComponent == null)
        {
            Debug.LogError("AttackSpawner: attackObj does not have Attack component!");
            return;
        }

        // TODO: this will be set indirectly via passing through attack context.
        //attackComponent.TargetTypes = targetTypes;
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

        AttackComponent ac = go.GetComponent<AttackComponent>();

        // Initialize movement
        if (movement == null)
        {
            Debug.LogError("AttackSpawner: Spawn: movement is null!");
            return null;
        }
        movement.Init(ac, data, context, amc);

        // Inject behaviors
        ac.Initialize(data, context, logic, movement);

        // Apply Attack Modifications
        if (ac.TryGetComponent<CircleScaler>(out var scaler))
        {
            scaler.SetCircleRadius(amc.ObjectScale);
        }

        ac.ResetAttackAfterTime(amc.AttackDuration);

        return ac;
    }
}
