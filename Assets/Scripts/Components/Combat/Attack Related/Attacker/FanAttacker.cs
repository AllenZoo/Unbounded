using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanAttacker : IAttacker
{
    public AttackerData AttackerData { get { return fanAttackerData; } set { fanAttackerData = (FanAttackerData)value; } }
    public AttackData AttackData { get { return attackData; } set { attackData = value; } }
    [OdinSerialize] private AttackData attackData;
    [OdinSerialize] private FanAttackerData fanAttackerData;

    private bool isAttacking = false;
    private MonoBehaviour coroutineRunner;
    private Coroutine curCoroutine;

    public FanAttacker() { }

    public FanAttacker(FanAttackerData fanAttackerData, AttackData attackData)
    {
        this.fanAttackerData = fanAttackerData;
        this.attackData = attackData;
    }

    public void Attack(KeyCode keyCode, AttackContext ac)
    {
        if (isAttacking)
        {
            return; // Attack already in progress
        }

        if (ac.AttackerComponent == null)
        {
            Debug.LogWarning("AttackerComponent is null. Cannot start attack coroutine.");
            return;
        }

        // If the coroutine runner has changed, update it
        if (coroutineRunner != ac.AttackerComponent)
        {
            // Stop any previous coroutines on the old runner if it exists
            if (coroutineRunner != null && curCoroutine != null)
            {
                // NOTE: this can be buggy if:
                //   1. multiple components share the same attacker. (Start and Stop conflicts..)
                coroutineRunner.StopCoroutine(curCoroutine);
            }
            coroutineRunner = ac.AttackerComponent;
        }

        // Start the attack coroutine
        curCoroutine = coroutineRunner.StartCoroutine(
            AttackCoroutine(
                ac.SpawnInfo, 
                ac.AttackerTransform, 
                ac.TargetTypes, 
                ac.AtkStat, 
                ac.PercentageDamageIncrease
                )
            );
    }

    public void StopAttack()
    {
        if (isAttacking && coroutineRunner != null && curCoroutine != null)
        {
            coroutineRunner.StopCoroutine(curCoroutine);
            isAttacking = false;
        }
    }

    private IEnumerator AttackCoroutine(AttackSpawnInfo info, Transform attackerTransform, List<EntityType> targetTypes, float atkStat, double percentageDamageIncrease)
    {
        isAttacking = true;
        Vector3 attackDir = info.mousePosition - attackerTransform.position;

        // Time delay between each wave (s)
        float delayBetweenWaves = fanAttackerData.timeBetweenBladeProjectiles;

        // Angle difference between each projectile spawned in a single wave (minimum of 1 degree)
        float spawnAngleDiff = Mathf.Max(1f, fanAttackerData.spinSpeed * delayBetweenWaves);

        // Angle difference between each blade in a single wave
        float angleSplit = 360f / fanAttackerData.numBlades;

        // Calculate total number of waves needed for a full 360-degree rotation
        int totalWaves = Mathf.CeilToInt(360f / spawnAngleDiff);

        // Direction multiplier for clockwise/counter-clockwise
        float direction = fanAttackerData.clockwiseSpin ? 1f : -1f;

        // Spawn waves
        for (int waveIndex = 0; waveIndex < totalWaves; waveIndex++)
        {
            float curSpawnAngleDiff = waveIndex * spawnAngleDiff * direction;
            Vector3 baseDir = Quaternion.Euler(0, 0, curSpawnAngleDiff) * attackDir;

            // Spawn all blades in this wave
            for (int i = 0; i < fanAttackerData.numBlades; i++)
            {
                float curAngleIncrement = i * angleSplit;
                Vector3 spawnDir = Quaternion.Euler(0, 0, curAngleIncrement) * baseDir;
                AttackSpawner.SpawnAttack(spawnDir, attackerTransform, targetTypes, attackData.attackPfb, this, atkStat, percentageDamageIncrease);
            }

            // Wait before spawning the next wave (but not after the last wave)
            if (waveIndex < totalWaves - 1)
            {
                yield return new WaitForSeconds(delayBetweenWaves);
            }
        }

        isAttacking = false;
    }

    public IAttacker DeepClone()
    {
        FanAttackerData clonedAttackerData = UnityEngine.Object.Instantiate(fanAttackerData);
        AttackData clonedAttackData = UnityEngine.Object.Instantiate(attackData);
        return new FanAttacker(clonedAttackerData, clonedAttackData);
    }

    public float GetChargeUp()
    {
        return fanAttackerData.chargeUp;
    }

    public float GetCooldown()
    {
        return fanAttackerData.cooldown;
    }

    public bool IsInitialized()
    {
        return fanAttackerData != null && attackData != null;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}