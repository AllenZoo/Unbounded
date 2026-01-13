using NUnit.Framework;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeteorAttacker : BaseAttacker<MeteorAttackerData>
{
    //public override AttackerData AttackerData { get { return meteorAttackerData; } set { meteorAttackerData = value; } }
    public override AttackData AttackData { get { return attackData; } set { attackData = value; } }
    [OdinSerialize] private AttackData attackData;
    [OdinSerialize] private MeteorAttackerData meteorAttackerData;

    [OdinSerialize] private IAttackIndicator attackIndicator; // Class that spawns an indicator of area where a meteor is going to land on.

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
            ac.SpawnInfo.mousePosition, 
            meteorAttackerData.errorRange, 
            meteorAttackerData.numAttacks);

        // For each meteor position, spawn an indicator and after a delay, spawn the meteor attack
        foreach (Vector3 pos in meteorPositions)
        {
            // Get random radius based on range. This is the final radius the indicator will grow to.
            float indicatorRadius = UnityEngine.Random.Range(
                meteorAttackerData.meteorRadiusRange.x,
                meteorAttackerData.meteorRadiusRange.y);

            // Initial Start Radius of indicator.
            float startRadius = indicatorRadius / 2;

            // (TODO: tweak radiusGrowthTime param). There is a paramter for this in AttackIndicatorData.cs, but we override that with this variable.
            float radiusGrowthTime = 1f;

            // Spawn indicator at pos
            attackIndicator.Indicate(new AttackIndicatorContext(pos, startRadius, indicatorRadius, true, radiusGrowthTime: radiusGrowthTime, growRadius: true));

            // After delay, spawn meteor attack at pos (TODO: implement delay and spawning)
            float indicatorTransitionTime = attackIndicator.Data.transitionTime;

            // TODO-OPT: Tweak Time To Target param.
            AttackSpawner.SpawnMeteorAttack(pos, timeToTarget: 1f, attackData.AttackPfb, meteorRadius: indicatorRadius, ac.TargetTypes);
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
        return meteorAttackerData.chargeUp;
    }
    public override float GetCooldown()
    {
        return meteorAttackerData.cooldown;
    }
    public override bool IsInitialized()
    {
        return attackData != null && meteorAttackerData != null && attackIndicator != null;
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
