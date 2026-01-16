using NUnit.Framework;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeteorAttacker : BaseAttacker<MeteorAttackerData>
{
    public override AttackData AttackData { get { return attackData; } set { attackData = value; } }
    [OdinSerialize] private AttackData attackData;

    [OdinSerialize] private IAttackIndicator attackIndicator; // Class that spawns an indicator of area where a meteor is going to land on.

    #region Meteor Attacker Custom Properties
    [FoldoutGroup("Meteor Attacker Custom Properties")]
    [SerializeField] private float timeToTarget = 1f;

    [FoldoutGroup("Meteor Attacker Custom Properties")]
    [Tooltip("Determines how long the residue of the attack stays for.")]
    [SerializeField] private float explosionAfterEffectDuration = 1f;

    [FoldoutGroup("Meteor Attacker Custom Properties")]
    [SerializeField] private float radiusGrowthTime = 1f;
    #endregion

    #region IAttacker Implementation
    /// <summary>
    /// Spawns an indicator, then a meteor attack to target.
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="ac"></param>
    public override void Attack(KeyCode keyCode, AttackContext ac)
    {
        // Randomly generate meteor positions within a certain area around the target position, given number of meteors to spawn, the error range,
        // and the target position
        List<Vector3> meteorPositions = CalculateMeteorPositions(
            ac.AttackSpawnInfo.targetPosition, 
            attackerData.errorRange, 
            attackerData.numAttacks);

        // For each meteor position, spawn an indicator and after a delay, spawn the meteor attack
        foreach (Vector3 pos in meteorPositions)
        {
            // Get random radius based on range. This is the final radius the indicator will grow to.
            float indicatorRadius = UnityEngine.Random.Range(
                attackerData.meteorRadiusRange.x,
                attackerData.meteorRadiusRange.y);

            // Initial Start Radius of indicator.
            float startRadius = indicatorRadius / 2;

            // Spawn indicator at pos
            attackIndicator.Indicate(new AttackIndicatorContext(pos, startRadius, indicatorRadius, true, radiusGrowthTime: radiusGrowthTime, growRadius: true));

            // After delay, spawn meteor attack at pos (TODO: implement delay and spawning)
            // Note: not deleting yet cause i have no clue what this used to do.
            float indicatorTransitionTime = attackIndicator.Data.transitionTime;

            ac.AttackSpawnInfo.targetPosition = pos; // Hack to position meteor at new location.
            var attackComponent = attackData?.AttackPfb?.GetComponent<AttackComponent>();
            if (attackComponent == null) Debug.LogError("Attack Pfb Does not contain Attack Component!");

            var amc = new AttackModificationContext
            {
                ObjectScale = indicatorRadius,
                AttackDuration = timeToTarget + explosionAfterEffectDuration
            };

            AttackSpawner.Spawn(attackData.AttackPfb, attackData, ac, amc, attackComponent.Attack, attackComponent.Movement);
        }
    }
    public override void StopAttack()
    {
        // Not a continouous attack so don't need.
    }
    public override IAttacker DeepClone()
    {
        throw new System.NotImplementedException();
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
        return attackData != null && attackerData != null && attackIndicator != null;
    }
    #endregion

    #region IAttacker Helpers
    /// <summary>
    /// Calculates random meteor spawn positions around a target.
    /// </summary>
    /// <param name="target"> The Transform whose position serves as the center point for meteor placement.</param>
    /// <param name="errorRange">
    /// A Vector2 defining the minimum and maximum random offset applied to the target 
    /// position on both the X and Y axes. (x = min offset, y = max offset).
    /// Each meteor's final position is shifted by a random value within this range,
    /// creating positional variation or spread.
    /// </param>
    /// <param name="meteorCount">The number of meteor positions to generate.</param>
    /// <returns>
    /// A list of Vector3 positions representing randomized meteor spawn points.
    /// </returns>

    private List<Vector3> CalculateMeteorPositions(Vector3 target, Vector2 errorRange, int meteorCount)
    {
        List<Vector3> positions = new List<Vector3>(meteorCount);

        for (int i = 0; i < meteorCount; i++)
        {
            float errorX = UnityEngine.Random.Range(errorRange.x, errorRange.y);
            float errorY = UnityEngine.Random.Range(errorRange.x, errorRange.y);

            Vector3 offset = new Vector3(errorX, errorY, 0f);
            Vector3 meteorPos = target + offset;

            positions.Add(meteorPos);
        }

        Debug.Assert(positions.Count == meteorCount,
            "Meteor count mismatch in CalculateMeteorPositions");

        return positions;
    }

    #endregion

    #region IAttackNode Implementation
    public override IEnumerable<IAttackNode> GetChildren()
    {
        yield return this;
    }
    #endregion
}
