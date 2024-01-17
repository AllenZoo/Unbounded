using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Handles enemy inputs such as movement and attacking.
[RequireComponent(typeof(StateComponent))]
public class EnemyAIComponent : InputController
{
    [SerializeField] protected CombatType combatType;
    
    #region Context Steering Related Variables
    [Tooltip("Used for calculating the best direction to move in to get to target.")]
    [SerializeField] protected ContextSteerer contextSteerer;

    [Tooltip("Position to spawn raycasts for detecting obstacles.")]
    [SerializeField] protected Transform feetTransform;

    [Tooltip("Used for keeping track of where to follow target.")]
    [SerializeField] protected ObjectTracker tracker;
    #endregion

    [Tooltip("Minimum distance to keep from target")]
    [SerializeField] protected float minDist = 0f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float movementTimer = 3f; // Time interval to change movement direction for random movement

    protected float timer;
    protected StateComponent state;
    protected GameObject aggroTarget;

    private delegate void AggroBehaviour(GameObject target);

    // Maps a combat type to a behaviour function.
    private Dictionary<CombatType, AggroBehaviour> behaviourMap;

    protected void Awake()
    {
        Assert.IsNotNull(contextSteerer, "contextSteerer must be assigned in inspector for AI to perform context steering movement.");
        Assert.IsNotNull(feetTransform, "feetTransform must be assigned in inspector for AI to perform context steering movement.");
        Assert.IsNotNull(tracker, "tracker must be assigned in inspector for AI to perform context steering movement.");

        Assert.IsTrue(minDist <= attackRange, "minDist must be less than or equal to attackRange");
        state = GetComponent<StateComponent>();

        // Init behaviour map
        behaviourMap = new Dictionary<CombatType, AggroBehaviour>
        {
            { CombatType.MELEE, Follow },
            { CombatType.RANGED, KiteTarget }
        };
    }

    protected void Start()
    {
        // Set the initial timer value
        timer = movementTimer;

        // Subscribe to state change event
        state.OnStateChanged += OnStateChange;
    }

    protected void Update()
    {
        // Count down the timer
        timer -= Time.deltaTime;

        // Check if enemy is aggroed to a target.
        if (aggroTarget != null)
        {
            // Aggroed. Perform behaviour based on combat type.
            behaviourMap[combatType](aggroTarget);
            
            // Attack if target is in range.
            ReadyAttack(aggroTarget, attackRange);
        } else
        {
            tracker.enabled = false;
            // TODO: undisable this eventually when we have a better way to handle this.
            // Random_Move();
        }
        
    }

    public void SetAggroTarget(GameObject target)
    {
        this.aggroTarget = target;
    }

    private void OnStateChange(State oldState, State newState)
    {

    }

    /// <summary>
    /// Randomly move around
    /// </summary>
    protected void Random_Move()
    {
        // If the timer reaches or goes below 0, change movement direction
        if (timer <= 0f)
        {
            // Randomly move (randX = -1/0/1, randY = -1/0/1)
            int randX = UnityEngine.Random.Range(-1, 2); // Include 2 to generate numbers from -1 to 1
            int randY = UnityEngine.Random.Range(-1, 2);

            Vector2 dir = new Vector2(randX, randY);

            // Invoke the event to notify listeners about the new motion direction
            base.InvokeMovementInput(dir);

            // Set state to WALKING (TODO: handle state in State component later)
            if (randX != 0 || randY != 0)
            {
                state.ReqStateChange(State.WALKING);
            }
            else
            {
                state.ReqStateChange(State.IDLE);
            }

            // Reset the timer
            timer = movementTimer;
        }
    }

    /// <summary>
    /// Move torwards a target, using context steering to avoid obstacles.
    /// </summary>
    /// <param name="target"></param>
    protected void Follow(GameObject target)
    {
        tracker.enabled = true;
        tracker.Track(target);

        // This is to prevent the enemy from stuttering and updating it's movement direction
        // too frequently
        if (timer >= 0f)
        {
            return;
        }

        state.ReqStateChange(State.RUNNING);

        // Use context steering to determine movement direction to best reach target.
        Vector2 dir = contextSteerer.GetDirTorwards(tracker.GetLastSeenTargetPos(), feetTransform.position);

        // Move towards the target
        base.InvokeMovementInput(dir);

        // Stutter the timer to prevent crazy movement.
        timer = 0.2f;
    }

    /// <summary>
    /// Move torwards a target until a suitable minDist, essentially kiting the target and keeping
    /// a safe distance from it. Uses context steering to avoid obstacles.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="minDist"> the minimum distance/space to keep from the target</param>
    protected void KiteTarget(GameObject target, float minDist)
    {
        tracker.enabled = true;
        tracker.Track(target);

        // This is to prevent the enemy from stuttering and updating it's movement direction
        // too frequently
        if (timer >= 0f)
        {
            return;
        }

        state.ReqStateChange(State.RUNNING);
        float dist = Vector2.Distance(transform.position, target.transform.position);

        // Initial direction to move torwards target
        Vector2 dir = contextSteerer.GetDirTorwards(tracker.GetLastSeenTargetPos(), feetTransform.position);

        if (dist < minDist)
        {
            // Move away from the target
            dir = contextSteerer.GetDirAway(tracker.GetLastSeenTargetPos(), feetTransform.position);
        }

        base.InvokeMovementInput(dir);

        // Stutter the timer to prevent crazy movement.
        timer = 0.2f;
    }

    /// <inheritdoc cref="KiteTarget(GameObject, float)"/>
    /// <summary>
    /// If no minDist is specified, use the default minDist serialized on entity.
    /// </summary>
    /// <param name="target"></param>
    protected void KiteTarget(GameObject target)
    {
        KiteTarget(target, minDist);
    }

    /// <summary>
    /// Track the distance from target, and attack when in range.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attackRange"></param>
    protected void ReadyAttack(GameObject target, float attackRange)
    {
        float dist = Vector2.Distance(transform.position, target.transform.position);
        // If the target is within attack range, attack
        if (dist < attackRange)
        {
            state.ReqStateChange(State.ATTACKING);
            Attack(target);
        }
    }

    /// <summary>
    /// Attack a target
    /// </summary>
    /// <param name="target"></param>
    protected void Attack(GameObject target)
    {
        // Invoke the event to notify listeners about the attack input

        base.InvokeAttackInput(KeyCode.K, new AttackSpawnInfo(target.transform.position));
    }
}



