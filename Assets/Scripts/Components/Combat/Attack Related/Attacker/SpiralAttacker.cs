using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralAttacker : BaseAttacker<SpiralAttackerData>
{
    public override AttackData AttackData { get { return attackData; } set { attackData = value; } }
    [Required, OdinSerialize] private AttackData attackData;

    private bool isAttacking = false;
    private MonoBehaviour coroutineRunner; // instance that runs the coroutine.
    private Coroutine curCoroutine; // keeps track of running coroutine. If null, this means no coroutine is currently running.

    public SpiralAttacker() { }

    public SpiralAttacker(SpiralAttackerData spiralAttackerData, AttackData attackData)
    {
        this.attackerData = spiralAttackerData;
        this.attackData = attackData;
    }

    #region IAttacker Implementation

    /// <summary>
    /// Spawns projectiles in a spiral like shape.
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="ac"></param>
    public override void Attack(KeyCode keyCode, AttackContext ac)
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
        curCoroutine = coroutineRunner.StartCoroutine(AttackCoroutine(ac));
    }

    public override void StopAttack()
    {
        isAttacking = false;

        if (coroutineRunner != null && curCoroutine != null)
        {
            coroutineRunner.StopCoroutine(curCoroutine);
        }

    }

    private IEnumerator AttackCoroutine(AttackContext ac)
    {
        isAttacking = true;
        Vector3 attackDir = ac.AttackSpawnInfo.targetPosition - ac.AttackerComponent.transform.position;

        // Time delay between each wave (s)
        float delayBetweenWaves = attackerData.timeBetweenBladeProjectiles;

        // Angle difference between each projectile spawned in a single wave (minimum of 1 degree)
        float spawnAngleDiff = Mathf.Max(1f, attackerData.spinSpeed * delayBetweenWaves);

        // Angle difference between each blade in a single wave
        float angleSplit = 360f / attackerData.numBlades;

        // Calculate total number of waves needed for a full 360-degree rotation
        int totalWaves = Mathf.CeilToInt(360f / spawnAngleDiff);

        // Direction multiplier for clockwise/counter-clockwise
        float direction = attackerData.clockwiseSpin ? 1f : -1f;

        // Spawn waves
        for (int waveIndex = 0; waveIndex < totalWaves; waveIndex++)
        {
            float curSpawnAngleDiff = waveIndex * spawnAngleDiff * direction;
            Vector3 baseDir = Quaternion.Euler(0, 0, curSpawnAngleDiff) * attackDir;

            // Spawn all blades in this wave
            for (int i = 0; i < attackerData.numBlades; i++)
            {
                float curAngleIncrement = i * angleSplit;

                Vector3 spawnDir = Quaternion.Euler(0, 0, curAngleIncrement) * baseDir;

                var attackComponent = attackData?.AttackPfb?.GetComponent<AttackComponent>();
                if (attackComponent == null)
                {
                    Debug.LogError("Attack Pfb Does not contain Attack Component!");
                    yield return new WaitForEndOfFrame();
                }

                AttackModificationContext amc = new AttackModificationContext() { 
                    AttackDuration = attackData.Distance / attackData.InitialSpeed,
                    AttackDirection = spawnDir.normalized
                    // Note: we don't need angle offset since attack direction already accounts for it
                };
                AttackSpawner.Spawn(attackData, ac, amc, attackComponent.Attack, attackComponent.Movement);
            }

            // Wait before spawning the next wave (but not after the last wave)
            if (waveIndex < totalWaves - 1)
            {
                yield return new WaitForSeconds(delayBetweenWaves);
            }
        }

        isAttacking = false;
    }

    public override IAttacker DeepClone()
    {
        SpiralAttackerData clonedAttackerData = UnityEngine.Object.Instantiate(attackerData);
        AttackData clonedAttackData = UnityEngine.Object.Instantiate(attackData);
        return new SpiralAttacker(clonedAttackerData, clonedAttackData);
    }

    public override float GetChargeUp()
    {
        return attackerData.chargeUp;
    }

    public override float GetCooldown()
    {
        return attackerData.cooldown;
    }

    public override bool IsInitialized()
    {
        return attackerData != null && attackData != null;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    #endregion

    #region IAttackNode Implementation
    public override IEnumerable<IAttackNode> GetChildren()
    {
        yield return this;
    }
    #endregion
}