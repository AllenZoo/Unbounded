using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "new EnemyAttack Rotate Patterns", menuName = "System/Enemy/State/Attack/AttackRotatePatterns")]
public class EnemyAttackRotatePatterns : EnemyAttackSOBase
{
    #region Behaviours
    [Tooltip("Defines the movement and attack pattern behaviour of entity.")]
    [OdinSerialize]
    private List<BehaviourDefinition> behaviours = new();
    private List<BehaviourDefinition> enabledBehaviours = new();

    [Required, OdinSerialize]
    private BehaviourDefinition emptyBehaviour;

    [OdinSerialize]
    private BehaviourDefinition rageBehaviour;

    [SerializeField]
    [Tooltip("HP threshold percentage to trigger rage mode. For example, if you want enemy to trigger rage at 20% hp, set this field to 0.2. If no rage mode, set to -1.")]
    private double rageModeHPThreshold = -1;
    private double curHPThreshold = 1; // keeps track of current enemy hp threshold.
    #endregion


    #region Transition Behaviour
    // Time range for how long a rotation should be active before being changed.
    [Tooltip("Time between each behaviour rotation in seconds.")]
    [SerializeField]
    private FloatRange transitionTimeRange;
    private float timer = 0f;

    [Tooltip("Time where boss does nothing for a bit to allow for player to react to next attack.")]
    [SerializeField]
    private float transitionPauseTime = 0f;
    #endregion


    [SerializeField]
    private bool debug = false;
    [OdinSerialize, ReadOnly, ShowIf(nameof(debug))] 
    private BehaviourDefinition currentBehaviour;


    // Local variable to help keep track of the likelihood of a behaviour being selected as the 'next' behaviour
    // Simple algorithm right now = assign a value of 1 to every behaviour. +1 if behaviour wasn't selected and reset 0 if it was.
    // TODO: figure out why Katan likes the trident sword slash so much???? The stats dont add up.
    private Dictionary<BehaviourDefinition, double> behaviourSelectionWeightMap = new Dictionary<BehaviourDefinition, double>();

    // To keep track of all the stat modifier references that we applied. Useful when we want to revert their effects.
    private List<StatModifier> appliedStatModifiers = new List<StatModifier>();

    #region SO Base functions
    public override void DoAnimationTriggerEventLogic()
    {
        base.DoAnimationTriggerEventLogic();
        // TODO: trigger attack animation.
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
            timer = UnityEngine.Random.Range(transitionTimeRange.min + transitionPauseTime, transitionTimeRange.max + transitionPauseTime);

            if (debug)
            {
                Debug.Log($"Behaviour Transition Timer Set to: {timer} seconds.");
            }
        }

        // Attack target.
        if (enemyAIComponent.AggroTarget != null) { 
            Transform targetTransform = enemyAIComponent.AggroTarget.transform;
            enemyAIComponent.InvokeAttackInput(KeyCode.K, new AttackSpawnInfo(targetTransform.position));
        }

        // If current behaviour is empty, we have no chase behaviour.
        if (currentBehaviour.Equals(emptyBehaviour))
        {
            // If not initialized we return.
            // NOTE: do not put this above the logic where we actually get and set the next behaviour since
            //       if we do, enemy will be permanently stuck in empty behaviour, and be empty forver :(
            return;
        }
        currentBehaviour.ChaseBehaviourInstance.DoFrameUpdateLogic(false);
    }

    public override void DoPhysicsUpdateLogic()
    {
        base.DoPhysicsUpdateLogic();

        if (currentBehaviour.Equals(emptyBehaviour))
        {
            // If not initialized we return.
            return;
        }
        currentBehaviour.ChaseBehaviourInstance.DoPhysicsUpdateLogic();
    }

    public override void Initialize(EnemyAIComponent enemyAIComponent, GameObject enemyObject, ContextSteerer contextSteerer, ObjectTracker tracker, Transform feetTransform, LocalEventHandler leh)
    {
        // Initialize self.
        base.Initialize(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform, leh);

        // Initalize all Behaviours
        foreach(var behaviour in behaviours)
        {
            behaviour.Init(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);
        }

        // Initialize Empty Behaviour
        emptyBehaviour?.Init(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);

        // Initalize Rage Behaviour (if not null)
        rageBehaviour?.Init(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);
   

        enabledBehaviours = behaviours.Where(b => !b.DisableBehaviour).ToList();

        ResetBehaviourSelectionWeightMap();
        currentBehaviour = emptyBehaviour;

        LocalEventBinding<OnStatChangeEvent> statChangeBinding = new LocalEventBinding<OnStatChangeEvent>(HandleHPThreshold);
        enemyAIComponent.LocalEventHandler.Register<OnStatChangeEvent>(statChangeBinding);
    }

    public override void ResetValues()
    {
        base.ResetValues();
        ResetBehaviourSelectionWeightMap();
        currentBehaviour = emptyBehaviour;
    }
    #endregion

    #region Helpers

    /// <summary>
    /// Initializes the behaviour selection weight map, assigning a weight of 1 to all behaviours.
    /// </summary>
    private void ResetBehaviourSelectionWeightMap()
    {
        if (debug)
        {
            Debug.Log("Resetting Behaviour Selection Weight Map.");
        }

        behaviourSelectionWeightMap.Clear();
        foreach(BehaviourDefinition behaviour in enabledBehaviours)
        {
            behaviourSelectionWeightMap.Add(behaviour, 1.0);
        }
    }

    /// <summary>
    /// Selects the next behaviour based on selection weight. The higher the weight, the higher the likelihood of selecting that behaviour. 
    /// When a behaviour is selected, reset its weight to zero.
    /// For every other behaviour not selected, add one to the weight. Max weight will be set to 5.
    /// 
    /// NOTE: If HP is below rage threshold, next behaviour will always be rage behaviour.
    /// 
    /// If any error occurs, returns currentBehaviour
    /// </summary>
    /// <returns>The selected BehaviourDefinition based on weights</returns>
    private BehaviourDefinition GetNextBehaviour()
    {
        if (curHPThreshold <= rageModeHPThreshold)
        {
            return rageBehaviour;
        }

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
                if (debug)
                {
                    Debug.Log($"Selected Behaviour is: {selectedBehaviour.Data.Name}");
                    Debug.Log($"Transitioning from {currentBehaviour.Data.Name} to {selectedBehaviour.Data.Name}.");
                }
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
                // Increment weight of non-selected behaviors, up to max of 5
                behaviourSelectionWeightMap[behaviour] = Math.Min(behaviourSelectionWeightMap[behaviour] + 1, 5);
            }
        }

        // Debug Updated Weights. Print out corresponding behaviour weight key value pair.
        if (debug)
        {
            string debugMsg = "Updated Behaviour Selection Weight Map \n";
            foreach (var kvp in behaviourSelectionWeightMap)
            {
                debugMsg += $"Behaviour [{kvp.Key.Data.Name}] has a weight of [{kvp.Value}] \n";
            }
            Debug.Log(debugMsg);
        }
       

        return selectedBehaviour;
    }

    /// <summary>
    /// Transitions from current behaviour to new behaviour.
    /// 
    /// Applies
    ///     1. Stat related changes (resets buffs, applies them, etc.)
    ///     2. New Attacker or Attack pattern
    ///     3. New movement behaviour. (currently this is automatically done once we set currentBehavior = newBehaviour, refer to where we call currentBehaviour.chaseBehaviourInstance.DoFrameUpdateLogic() etc.)
    ///    
    /// NOTE: If we are transitioning into same behaviour, simply returns. Good for handling repeated rage phase behaviour.
    /// </summary>
    /// <param name="newBehaviour"></param>
    private void TransitionBehaviour(BehaviourDefinition newBehaviour) {
        var prevBehaviour = currentBehaviour;
        
        if (prevBehaviour.Data.Name.Equals(newBehaviour.Data.Name))
        {
            return;
        }
        currentBehaviour = newBehaviour;

        if (debug)
        {
            Debug.Log($"Transitioning to new behaviour: [{newBehaviour.Data.Name}]");
        }

        // 1a. Revert previous stat changes
        foreach (StatModifier statModifier in appliedStatModifiers)
        {
            statModifier.Dispose();
        }
        
        // 1b. Apply new stat changes
        if (currentBehaviour.Data.AddStatModifiers != null)
        {
            foreach (AddStatModifier addStatModifier in currentBehaviour.Data.AddStatModifiers)
            {
                var statModifier = new StatModifier(addStatModifier.stat, new AddOperation(addStatModifier.amount), -1);
                enemyAIComponent.LocalEventHandler.Call(new OnStatBuffEvent { buff = statModifier });
                appliedStatModifiers.Add(statModifier);
            }
        }
        

        // 2. Set Attacker
        if (enemyAIComponent.AttackerComponent != null)
        {
            enemyAIComponent.AttackerComponent.PauseAttacker(transitionPauseTime);
            enemyAIComponent.AttackerComponent.SetAttacker(new List<IAttacker>() { currentBehaviour.Attacker });
        }
    }

    /// <summary>
    /// Helper to calculate the current HP threshold. Called whenever OnStatChangeEvent is called.
    /// </summary>
    /// <param name="statChangeEvent"></param>
    private void HandleHPThreshold(OnStatChangeEvent statChangeEvent)
    {
        StatComponent statComponent = statChangeEvent.statComponent;

        if (statComponent != null)
        {
            // Calculate HP threshold percentage
            float maxHP = statComponent.StatContainer.MaxHealth;
            float curHP = statComponent.StatContainer.Health;

            double percentage = curHP / maxHP;
            curHPThreshold = percentage;

            // Immediately transition to rage phase if threshold is breached and we are not already in rage
            if (rageBehaviour != null && curHPThreshold <= rageModeHPThreshold)
            {
                if (currentBehaviour == null || !currentBehaviour.Data.Name.Equals(rageBehaviour.Data.Name))
                {
                    if (debug)
                    {
                        Debug.Log("HP below threshold! Immediately transitioning to Rage Phase.");
                    }
                    TransitionBehaviour(rageBehaviour);
                    // Reset timer to full duration of a rotation when entering rage. 
                    // Note: this doesn't really matter since the next behaviour after rage will always be rage until death,
                    // but we do this just for consistency and to avoid any potential weird edge cases. 
                    timer = UnityEngine.Random.Range(transitionTimeRange.min + transitionPauseTime, transitionTimeRange.max + transitionPauseTime);
                }
            }
            
            if (debug)
            {
                Debug.Log($"Current HP Threshold: {curHPThreshold}");
            }
        }
    }
    #endregion
}

#region Classes/Structs for Serialization
[Serializable]
public class BehaviourDefinition
{
     public BehaviourDefinitionData Data { get { return data; } private set { } }  // Initial Static Data
    [Required, SerializeField] private BehaviourDefinitionData data;

    [ReadOnly] public EnemyChaseSOBase ChaseBehaviourInstance; // We should initialize this ref and not the above, since SO are shared. We need to create new ref using Instantiate.

    [OdinSerialize, ReadOnly]
    private IAttacker clonedAttacker;
    public IAttacker Attacker => clonedAttacker ?? Data.Attacker;

    public bool DisableBehaviour { get { return disableBehaviour; } private set { } }

    [Header("Debugging")]
    [SerializeField, Tooltip("Disable behaviour for debugging")]
    private bool disableBehaviour = false;

    public class Builder
    {
        private BehaviourDefinitionData data;
        public Builder WithBuilderData(BehaviourDefinitionData data)
        {
            this.data = data;
            return this;
        }
        public Builder WithoutBuilderData()
        {
            return this;
        }

        public BehaviourDefinition Build()
        {
            return new BehaviourDefinition() { Data = data };
        }

    }


    public void Init(EnemyAIComponent enemyAIComponent, GameObject enemyObject, ContextSteerer contextSteerer, ObjectTracker tracker, Transform feetTransform)
    {
        clonedAttacker = Data.Attacker?.DeepClone();

        if (Data.ChaseBehaviour != null)
        {
            ChaseBehaviourInstance = UnityEngine.Object.Instantiate(Data.ChaseBehaviour);
            ChaseBehaviourInstance.Initialize(enemyAIComponent, enemyObject, contextSteerer, tracker, feetTransform);
        } else
        {
            Debug.LogWarning($"Chase Behaviour for {Data.Name} is null.");
        }
        
    }
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