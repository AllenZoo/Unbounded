using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Handles enemy inputs such as movement and attacking.
[RequireComponent(typeof(StateComponent))]
public class EnemyAIComponent : InputController
{   
    #region Context Steering Related Variables
    [Tooltip("Used for calculating the best direction to move in to get to target.")]
    [Required]
    [SerializeField] protected ContextSteerer contextSteerer;

    [Tooltip("Position to spawn raycasts for detecting obstacles.")]
    [Required]
    [SerializeField] protected Transform feetTransform;

    [Tooltip("Used for keeping track of where to follow target.")]
    [Required]
    [SerializeField] protected ObjectTracker tracker;
    #endregion

    public AttackerComponent AttackerComponent { get; private set; }
    [Required][SerializeField] protected AttackerComponent attackerComponent;
    public float AttackRange { get { return attackRange; } private set { } }
    [SerializeField] protected float attackRange = 2f;

    public GameObject AggroTarget { get { return aggroTarget; } private set { } }
    protected GameObject aggroTarget;


    #region State Machine Variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState EnemyIdleState { get; set; }
    public EnemyChaseState EnemyChaseState { get; set; }
    public EnemyAttackState EnemyAttackState { get; set; }

    [Required]
    [SerializeField] private EnemyIdleSOBase enemyIdleBase;

    [Required]
    [SerializeField] private EnemyChaseSOBase enemyChaseBase;

    [Required]
    [SerializeField] private EnemyAttackSOBase enemyAttackBase;

    public EnemyIdleSOBase EnemyIdleBaseInstance { get;  set; }
    public EnemyChaseSOBase EnemyChaseBaseInstance { get; set; }
    public EnemyAttackSOBase EnemyAttackBaseInstance { get;  set; }
    #endregion


    protected void Awake()
    {
        base.Awake();
        Assert.IsNotNull(contextSteerer, "contextSteerer must be assigned in inspector for AI to perform context steering movement.");
        Assert.IsNotNull(feetTransform, "feetTransform must be assigned in inspector for AI to perform context steering movement.");
        Assert.IsNotNull(tracker, "tracker must be assigned in inspector for AI to perform context steering movement.");

        // Init State Machine Vars
        StateMachine = new EnemyStateMachine();
        EnemyIdleState = new EnemyIdleState(this, StateMachine);
        EnemyChaseState = new EnemyChaseState(this, StateMachine);
        EnemyAttackState = new EnemyAttackState(this, StateMachine);
        

        EnemyIdleBaseInstance = Instantiate(enemyIdleBase);
        EnemyChaseBaseInstance = Instantiate(enemyChaseBase);
        EnemyAttackBaseInstance = Instantiate(enemyAttackBase);
    }

    protected void Start()
    {
        // Init State Machine SO Variables
        EnemyIdleBaseInstance.Initialize(this, gameObject);
        EnemyChaseBaseInstance.Initialize(this, gameObject, contextSteerer, tracker, feetTransform);
        EnemyAttackBaseInstance.Initialize(this, gameObject, contextSteerer, tracker, feetTransform);

        StateMachine.Initialize(EnemyIdleState);

        // Subscribe to state change event
        LocalEventBinding<OnStateChangeEvent> stateChangeBinding = new LocalEventBinding<OnStateChangeEvent>(OnStateChange);
        localEventHandler.Register(stateChangeBinding);
    }

    protected void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    protected void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }

    public void SetAggroTarget(GameObject target)
    {
        this.aggroTarget = target;
    }

    private void OnStateChange(OnStateChangeEvent e)
    {

    }
}



