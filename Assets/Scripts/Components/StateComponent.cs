using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keeps track of the state of the gameobject and uses that info to perform 
// some logic.
public class StateComponent : MonoBehaviour
{
    [SerializeField] private LocalEventHandler localEventHandler;
    public State state { get; private set; } = State.IDLE;

    [Header("For debugging, doesn't affect anything.")]
    [SerializeField, ReadOnly] State debuggingState = State.IDLE;
    [SerializeField, ReadOnly] private List<State> crowdControlStates = new List<State>() {State.STUNNED };

    private delegate IEnumerator AnimationCoroutine();
    private delegate void StateAction();
    private Coroutine deactivationCoroutine;

    private void Awake()
    {
        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                    "] with root object [" + gameObject.transform.root.name + "] for StateComponent.cs");
            }
        }
    }

    private void Start()
    {
        LocalEventBinding<OnStateChangeEvent> stateChangeBinding = new LocalEventBinding<OnStateChangeEvent>(HandleStateChanged);
        localEventHandler.Register(stateChangeBinding);

        LocalEventBinding<OnKnockBackBeginEvent> knockBackBeginBinding = new LocalEventBinding<OnKnockBackBeginEvent>(HandleKnockBackBeginEvent);
        localEventHandler.Register(knockBackBeginBinding);

        LocalEventBinding<OnKnockBackEndEvent> knockBackEndBinding = new LocalEventBinding<OnKnockBackEndEvent>(HandleKnockBackEndEvent);
        localEventHandler.Register(knockBackEndBinding);

        LocalEventBinding<OnDeathEvent> onDeathBinding = new LocalEventBinding<OnDeathEvent>(HandleOnDeathEvent);
        localEventHandler.Register(onDeathBinding);

        LocalEventBinding<OnMovementInput> onMovementInputBinding = new LocalEventBinding<OnMovementInput>(HandleMovementInputEvent);
        localEventHandler.Register(onMovementInputBinding);
    }

    // If current state is not part of a CC state (crowd controlled state), then change state.
    // Otherwise, a CC state can only be changed into CCFREE state, or into a higher priority CC state.
    // If dead, cannot switch to any other state. (unless maybe new State 'Revived' we add)
    public void ReqStateChange(State newState)
    {
        if (newState == state)
        {
            return;
        }

        if (state == State.DEAD)
        {
            return;
        }

        if (crowdControlStates.Contains(state))
        {
            if (newState == State.CCFREE)
            {
                SetState(newState);
            }
        }
        else
        {
            SetState(newState);
        }
    }

    public List<State> GetCCStates
    {
        get { return crowdControlStates; }
    }

    /// <summary>
    /// Forces the state back to IDLE, allowing a dead entity to be revived.
    /// </summary>
    public void ResetState()
    {
        if (deactivationCoroutine != null) StopCoroutine(deactivationCoroutine);
        SetState(State.IDLE);
    }

    private void HandleOnDeathEvent(OnDeathEvent e)
    {
        ReqStateChange(State.DEAD);
    }
    private void HandleMovementInputEvent(OnMovementInput i)
    {
        // Set state based on movement input.
        // TODO: might want to seperate states of different categories:
        //    MovementStates, CombatStates, InteractionStates, etc.
        if (i.movementInput.x != 0 || i.movementInput.y != 0)
        {
            ReqStateChange(State.WALKING);
        }
        else
        {
            ReqStateChange(State.IDLE);
        }
    }
    private void HandleStateChanged(OnStateChangeEvent e)
    {
        // TODO: handle destroying enemy somewhere else. Maybe even make a pool!
        if (e.newState == State.DEAD)
        {
            // Destroy(gameObject, 1.0f);
            StartCoroutine(WaitThenCall(DeactivateEntity, 1.0f));
        }
    }
    private void HandleKnockBackBeginEvent(OnKnockBackBeginEvent e)
    {
        ReqStateChange(State.STUNNED);
    }
    private void HandleKnockBackEndEvent(OnKnockBackEndEvent e)
    {
        ReqStateChange(State.CCFREE);
    }

    private void SetState(State state)
    {
        State oldState = this.state;
        this.state = state;
        localEventHandler.Call(new OnStateChangeEvent { newState = state, oldState = oldState });

        // Debugging
        debuggingState = state;
    }

    
    private IEnumerator WaitForClipEnd(AnimationCoroutine coroutine, StateAction onEnd)
    {
        yield return StartCoroutine(coroutine());
        onEnd();
        // ReqStateChange(State.CCFREE);
    }
    private IEnumerator WaitThenCall (StateAction onEnd, float time)
    {
        yield return new WaitForSeconds(time);
        onEnd();
    }

    private void DeactivateEntity()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
    }
}
