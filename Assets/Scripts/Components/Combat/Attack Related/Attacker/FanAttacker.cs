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

    public FanAttacker() { }

    public FanAttacker(FanAttackerData fanAttackerData, AttackData attackData)
    {
        this.fanAttackerData = fanAttackerData;
        this.attackData = attackData;
    }

    public void SetCoroutineRunner(MonoBehaviour runner)
    {
        coroutineRunner = runner;
    }

    public void Attack(KeyCode keyCode, AttackSpawnInfo info, AttackerComponent attackerComponent, Transform attackerTransform, List<EntityType> targetTypes, float atkStat, double percentageDamageIncrease)
    {
        if (isAttacking)
        {
            return; // Attack already in progress
        }

        if (attackerComponent == null)
        {
            Debug.LogWarning("AttackerComponent is null. Cannot start attack coroutine.");
            return;
        }

        // If the coroutine runner has changed, update it
        if (coroutineRunner != attackerComponent)
        {
            // Stop any previous coroutines on the old runner if it exists
            if (coroutineRunner != null)
            {
                // NOTE: this can be buggy if:
                //   1. multiple attackers share the same component
                //   2. other coroutines are running on the same component
                coroutineRunner.StopAllCoroutines();
            }
            coroutineRunner = attackerComponent;
        }

        // Start the attack coroutine
        coroutineRunner.StartCoroutine(AttackCoroutine(info, attackerTransform, targetTypes, atkStat, percentageDamageIncrease));
    }

    private IEnumerator AttackCoroutine(AttackSpawnInfo info, Transform attackerTransform, List<EntityType> targetTypes, float atkStat, double percentageDamageIncrease)
    {
        isAttacking = true;

        Vector3 attackDir = info.mousePosition - attackerTransform.position;
        float spawnAngleDiff = 5f; // Angle difference between each projectile spawned in a single blade
        float angleSplit = 360f / fanAttackerData.numBlades; // Angle difference between each blade
        float curSpawnAngleDiff = 0;
        float delayBetweenWaves = 0.05f; // Time delay between each wave (adjustable) TODO: use projectileDensity from FanAttackerData later.

        // Cycle until spawn angle diff reaches one full cycle
        while (curSpawnAngleDiff < 360f)
        {
            float curAngleIncrement = 0;
            Vector3 baseDir = Quaternion.Euler(0, 0, curSpawnAngleDiff) * attackDir;

            for (int i = 0; i < fanAttackerData.numBlades; i++)
            {
                Vector3 spawnDir = Quaternion.Euler(0, 0, curAngleIncrement) * baseDir;
                AttackSpawner.SpawnAttack(spawnDir, attackerTransform, targetTypes, attackData.attackPfb, this, atkStat, percentageDamageIncrease);

                // Increment for next blade
                curAngleIncrement += angleSplit;
            }

            curSpawnAngleDiff += spawnAngleDiff;

            // Wait before spawning the next wave
            yield return new WaitForSeconds(delayBetweenWaves);
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