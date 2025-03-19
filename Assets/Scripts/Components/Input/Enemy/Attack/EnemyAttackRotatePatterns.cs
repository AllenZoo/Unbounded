using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "new EnemyAttack Rotate Patterns", menuName = "System/Enemy/State/Attack/AttackRotatePatterns")]
public class EnemyAttackRotatePatterns : EnemyAttackSOBase
{
    [Tooltip("Defines the movement and attack pattern behaviour of entity.")]
    [SerializeField]
    [OdinSerialize]
    private List<BehaviourDefinition> behaviours = new();

    // Time range for how long a rotation should be active before being changed.
    [Tooltip("Time between each behaviour rotation in seconds.")]
    [SerializeField]
    [OdinSerialize]
    private FloatRange transitionTimeRange;
    private float timer = 0f;

    private BehaviourDefinition currentBehaviour;

    // Local variable to help keep track of the likelihood of a behaviour being selected as the 'next' behaviour
    // Simple algorithm right now = assign a value of 1 to every behaviour. +1 if behaviour wasn't selected and reset 0 if it was.
    private Dictionary<BehaviourDefinition, double> behaviourSelectionWeightMap;

    // To keep track of all the stat modifier references that we applied. Useful when we want to revert their effects.
    private List<StatModifier> appliedStatModifiers = new List<StatModifier>();

    #region SO Base functions
    public override void DoAnimationTriggerEventLogic()
    {
        base.DoAnimationTriggerEventLogic();
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoFrameUpdateLogic()
    {
        base.DoFrameUpdateLogic();

        // Behaviour Rotation Logic
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // Once timer is up, we transition behaviour.
            var nextBehaviour = GetNextBehaviour();
            TransitionBehaviour(nextBehaviour);
            timer = UnityEngine.Random.Range(transitionTimeRange.min, transitionTimeRange.max);
        }

        // Attack
        if (enemyAIComponent.AggroTarget != null) { 
            Transform targetTransform = enemyAIComponent.AggroTarget.transform;
            enemyAIComponent.InvokeAttackInput(KeyCode.K, new AttackSpawnInfo(targetTransform.position));
        }

        currentBehaviour.chaseBehaviour.DoFrameUpdateLogic();
    }

    public override void DoPhysicsUpdateLogic()
    {
        base.DoPhysicsUpdateLogic();
        currentBehaviour.chaseBehaviour.DoPhysicsUpdateLogic();
    }

    public override void Initialize(EnemyAIComponent enemyAIComponent, GameObject enemyObject, ContextSteerer contextSteerer, ObjectTracker tracker, Transform feetTransform)
    {
        base.Initialize(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);

        // Initalize all possible ChaseSOBase Behaviours
        foreach(var behaviour in behaviours)
        {
            behaviour.chaseBehaviour.Initialize(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);
        }

        ResetBehaviourSelectionWeightMap();
        // Temporary fix for currentBehaviour being null when initialized.
        currentBehaviour = behaviours[0];
    }

    public override void ResetValues()
    {
        base.ResetValues();
        ResetBehaviourSelectionWeightMap();
        // Temporary fix for currentBehaviour being null when initialized.
        currentBehaviour = behaviours[0];
    }
    #endregion

    #region Helpers

    /// <summary>
    /// Initializes the behaviour selection weight map, assigning a weight of 1 to all behaviours.
    /// </summary>
    private void ResetBehaviourSelectionWeightMap()
    {
        behaviourSelectionWeightMap = new Dictionary<BehaviourDefinition, double>();
        foreach(BehaviourDefinition behaviour in behaviours)
        {
            behaviourSelectionWeightMap.Add(behaviour, 1.0);
        }
    }

    /// <summary>
    /// Selects the next behaviour based on selection weight. The higher the weight, the higher the likelihood of selecting that behaviour. 
    /// When a behaviour is selected, reset its weight to zero.
    /// For every other behaviour not selected, add one to the weight. Max weight will be set to 3.
    /// 
    /// If any error occurs, returns currentBehaviour
    /// </summary>
    /// <returns>The selected BehaviourDefinition based on weights</returns>
    private BehaviourDefinition GetNextBehaviour()
    {
        // Ensure the weight map is initialized
        if (behaviourSelectionWeightMap == null || behaviourSelectionWeightMap.Count == 0)
        {
            // Return current behavior if no behaviors exist
            return currentBehaviour;
        }

        // Calculate total weight
        double totalWeight = 0;
        foreach (var weight in behaviourSelectionWeightMap.Values)
        {
            totalWeight += weight;
        }

        // If all weights are zero, we can't select based on weight
        if (totalWeight <= 0)
        {
            // Default to equal probability
            totalWeight = behaviourSelectionWeightMap.Count;
        }

        // Random value between 0 and total weight
        double randomValue = new System.Random().NextDouble() * totalWeight;

        // Find the selected behavior
        BehaviourDefinition selectedBehaviour = currentBehaviour;
        double cumulativeWeight = 0;

        foreach (var kvp in behaviourSelectionWeightMap)
        {
            // If weight is zero, skip behaviour
            if (kvp.Value == 0) {
                continue;
            }

            cumulativeWeight += kvp.Value;
            if (randomValue <= cumulativeWeight)
            {
                selectedBehaviour = kvp.Key;
                break;
            }
        }

        // Update weights
        foreach (var behaviour in behaviourSelectionWeightMap.Keys.ToList())
        {
            if (behaviour.Equals(selectedBehaviour))
            {
                // Reset weight of selected behavior
                behaviourSelectionWeightMap[behaviour] = 0;
            }
            else
            {
                // Increment weight of non-selected behaviors, up to max of 3
                behaviourSelectionWeightMap[behaviour] = Math.Min(behaviourSelectionWeightMap[behaviour] + 1, 3);
            }
        }

        return selectedBehaviour;
    }

    /// <summary>
    /// Transitions from current behaviour to new behaviour.
    /// 
    /// Applies
    ///     1. Stat related changes (resets buffs, applies them, etc.)
    ///     2. New Attacker or Attack pattern
    ///     3. New movement behaviour. (currently this is automatically done once we set currentBehavior = newBehaviour, refer to where we call currentBehaviour.chaseBehaviour.DoFrameUpdateLogic() etc.)
    /// </summary>
    /// <param name="newBehaviour"></param>
    private void TransitionBehaviour(BehaviourDefinition newBehaviour) {
        var prevBehaviour = currentBehaviour;
        currentBehaviour = newBehaviour;

        // 1a. Revert previous stat changes
        foreach (StatModifier statModifier in appliedStatModifiers)
        {
            statModifier.Dispose();
        }
        
        // 1b. Apply new stat changes
        foreach (AddStatModifier addStatModifier in currentBehaviour.addStatModifiers)
        {
            var statModifier = new StatModifier(addStatModifier.stat, new AddOperation(addStatModifier.amount), -1);
            enemyAIComponent.LocalEventHandler.Call(new OnStatBuffEvent { buff = statModifier });
            appliedStatModifiers.Add(statModifier);
        }

        // 2. Set Attacker
        if (enemyAIComponent.AttackerComponent != null)
        {
            enemyAIComponent.AttackerComponent.SetAttacker(currentBehaviour.attacker);
        }
    }
    #endregion
}

#region Structs for Serialization
[Serializable]
public struct BehaviourDefinition
{
    public Attacker attacker;
    public EnemyChaseSOBase chaseBehaviour;
    public List<AddStatModifier> addStatModifiers;
}

[Serializable]
public struct AddStatModifier
{
    public Stat stat;
    public float amount;
}

[Serializable]
public struct FloatRange
{
    public float min;
    public float max;
}
#endregion