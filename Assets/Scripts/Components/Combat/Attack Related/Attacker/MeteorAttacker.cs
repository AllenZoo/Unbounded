using NUnit.Framework;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeteorAttacker : IAttacker
{
    public AttackerData AttackerData { get { return meteorAttackerData; } set { meteorAttackerData = (MeteorAttackerData) value; } }
    public AttackData AttackData { get { return attackData; } set { attackData = value; } }
    [OdinSerialize] private AttackData attackData;
    [OdinSerialize] private MeteorAttackerData meteorAttackerData;

    // TODO: create an indicator class (creates indicator that transitions from transparent to opaque over time)
    [OdinSerialize]
    private IAttackIndicator attackIndicator;

    #region IAttacker Implementation
    public void Attack(KeyCode keyCode, AttackContext attackContext)
    {

        // Randomly generate meteor positions within a certain area around the target position, given number of meteors to spawn, the error range,
        // and the target position (TODO: currently providing transform of the attacker instaead of target).
        List<Vector3> meteorPositions = CalculateMeteorPositions(
            attackContext.SpawnInfo.mousePosition, 
            meteorAttackerData.errorRange, 
            meteorAttackerData.numAttacks);

        // TODO: for each meteor position, spawn an indicator and after a delay, spawn the meteor attack
        foreach (Vector3 pos in meteorPositions)
        {
            // Get random radius based on range
            float indicatorRadius = UnityEngine.Random.Range(
                meteorAttackerData.meteorRadiusRange.x,
                meteorAttackerData.meteorRadiusRange.y);

            // Spawn indicator at pos
            attackIndicator.Indicate(new AttackIndicatorContext(pos, indicatorRadius, true));

            // After delay, spawn meteor attack at pos (TODO: implement delay and spawning)
            float indicatorTransitionTime = attackIndicator.Data.transitionTime;

            AttackSpawner.SpawnMeteorAttack(pos, 1f, attackData.attackPfb);
        }
    }
    public void StopAttack()
    {
        //throw new System.NotImplementedException();
    }
    public IAttacker DeepClone()
    {
        throw new System.NotImplementedException();
    }
    public float GetChargeUp()
    {
        return meteorAttackerData.chargeUp;
    }
    public float GetCooldown()
    {
        return meteorAttackerData.cooldown;
    }
    public bool IsInitialized()
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

}
