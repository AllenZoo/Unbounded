using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralAttacker : IAttacker, IAttackNode
{
    public AttackerData AttackerData { get { return spiralAttackerData; } set { spiralAttackerData = (SpiralAttackerData)value; } }
    public AttackData AttackData { get { return attackData; } set { attackData = value; } }
    [OdinSerialize] private AttackData attackData;
    [OdinSerialize] private SpiralAttackerData spiralAttackerData;

    private bool isAttacking = false;
    private MonoBehaviour coroutineRunner; // instance that runs the coroutine.
    private Coroutine curCoroutine; // keeps track of running coroutine. If null, this means no coroutine is currently running.

    public SpiralAttacker() { }

    public SpiralAttacker(SpiralAttackerData fanAttackerData, AttackData attackData)
    {
        this.spiralAttackerData = fanAttackerData;
        this.attackData = attackData;
    }

    /// <summary>
    /// Spawns projectiles in a spiral like shape.
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="ac"></param>
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
        isAttacking = false;

        if (coroutineRunner != null && curCoroutine != null)
        {
            coroutineRunner.StopCoroutine(curCoroutine);
        }

    }

    private IEnumerator AttackCoroutine(AttackSpawnInfo info, Transform attackerTransform, List<EntityType> targetTypes, float atkStat, double percentageDamageIncrease)
    {
        isAttacking = true;
        Vector3 attackDir = info.mousePosition - attackerTransform.position;

        // Time delay between each wave (s)
        float delayBetweenWaves = spiralAttackerData.timeBetweenBladeProjectiles;

        // Angle difference between each projectile spawned in a single wave (minimum of 1 degree)
        float spawnAngleDiff = Mathf.Max(1f, spiralAttackerData.spinSpeed * delayBetweenWaves);

        // Angle difference between each blade in a single wave
        float angleSplit = 360f / spiralAttackerData.numBlades;

        // Calculate total number of waves needed for a full 360-degree rotation
        int totalWaves = Mathf.CeilToInt(360f / spawnAngleDiff);

        // Direction multiplier for clockwise/counter-clockwise
        float direction = spiralAttackerData.clockwiseSpin ? 1f : -1f;

        // Spawn waves
        for (int waveIndex = 0; waveIndex < totalWaves; waveIndex++)
        {
            float curSpawnAngleDiff = waveIndex * spawnAngleDiff * direction;
            Vector3 baseDir = Quaternion.Euler(0, 0, curSpawnAngleDiff) * attackDir;

            // Spawn all blades in this wave
            for (int i = 0; i < spiralAttackerData.numBlades; i++)
            {
                float curAngleIncrement = i * angleSplit;
                Vector3 spawnDir = Quaternion.Euler(0, 0, curAngleIncrement) * baseDir;
                AttackSpawner.SpawnAttackInPool(spawnDir, attackerTransform, targetTypes, attackData.AttackPfb, this, atkStat, percentageDamageIncrease);
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
        SpiralAttackerData clonedAttackerData = UnityEngine.Object.Instantiate(spiralAttackerData);
        AttackData clonedAttackData = UnityEngine.Object.Instantiate(attackData);
        return new SpiralAttacker(clonedAttackerData, clonedAttackData);
    }

    public float GetChargeUp()
    {
        return spiralAttackerData.chargeUp;
    }

    public float GetCooldown()
    {
        return spiralAttackerData.cooldown;
    }

    public bool IsInitialized()
    {
        return spiralAttackerData != null && attackData != null;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    #region IAttackNode Implementation
    public IEnumerable<IAttackNode> GetChildren()
    {
        yield return this;
    }
    #endregion
}