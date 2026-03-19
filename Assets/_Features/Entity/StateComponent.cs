using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keeps track of the state of the gameobject and uses that info to perform 
// some logic.
public class StateComponent : MonoBehaviour
{
    public TrackedEvent OnStateChanged = new TrackedEvent("OnStateChanged");
    [SerializeField] private LocalEventHandler leh;
    public State State { get; private set; } = State.IDLE;

    [Header("For debugging, doesn't affect anything.")]
    [SerializeField, ReadOnly] State debuggingState = State.IDLE;
    [SerializeField, ReadOnly] private List<State> crowdControlStates = new List<State>() {State.STUNNED };

    private delegate IEnumerator AnimationCoroutine();
    private delegate void StateAction();
    private Coroutine deactivationCoroutine;

    [SerializeField] private float damageStateDuration = 0.5f;
    private Coroutine damageResetCoroutine;


    #region Local Event Bindings
    LocalEventBinding<OnStateChangeEvent> stateChangeBinding;
    LocalEventBinding<OnKnockBackBeginEvent> knockBackBeginBinding;
    LocalEventBinding<OnKnockBackEndEvent> knockBackEndBinding;
    LocalEventBinding<OnDeathEvent> onDeathBinding;
    LocalEventBinding<OnMovementInput> onMovementInputBinding;
    LocalEventBinding<OnDamagedEvent> onDamagedBinding;
    #endregion


    private void Awake()
    {
        leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);
        if (leh == null) Debug.LogError("StateComponent on " + gameObject.name + " couldn't find LocalEventHandler in parents.");

        stateChangeBinding = new LocalEventBinding<OnStateChangeEvent>(HandleStateChanged);
        knockBackBeginBinding = new LocalEventBinding<OnKnockBackBeginEvent>(HandleKnockBackBeginEvent);
        knockBackEndBinding = new LocalEventBinding<OnKnockBackEndEvent>(HandleKnockBackEndEvent);
        onDeathBinding = new LocalEventBinding<OnDeathEvent>(HandleOnDeathEvent);
        onMovementInputBinding = new LocalEventBinding<OnMovementInput>(HandleMovementInputEvent);
        onDamagedBinding = new LocalEventBinding<OnDamagedEvent>(HandleOnDamagedEvent);
    }

    private void OnEnable()
    {
        leh.Register(stateChangeBinding);
        leh.Register(knockBackBeginBinding);
        leh.Register(knockBackEndBinding);
        leh.Register(onDeathBinding);
        leh.Register(onMovementInputBinding);
        leh.Register(onDamagedBinding);
    }

    private void OnDisable()
    {
        leh.Unregister(stateChangeBinding);
        leh.Unregister(knockBackBeginBinding);
        leh.Unregister(knockBackEndBinding);
        leh.Unregister(onDeathBinding);
        leh.Unregister(onMovementInputBinding);
        leh.Unregister(onDamagedBinding);
    }

    // If current state is not part of a CC state (crowd controlled state), then change state.
    // Otherwise, a CC state can only be changed into CCFREE state, or into a higher priority CC state.
    // If dead, cannot switch to any other state. (unless maybe new State 'Revived' we add)
    public void ReqStateChange(State newState)
    {
        if (newState == State)
        {
            return;
        }

        if (State == State.DEAD)
        {
            return;
        }

        if (crowdControlStates.Contains(State))
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
    private void HandleOnDamagedEvent(OnDamagedEvent e)
    {
        ReqStateChange(State.DAMAGED);

        // TODO: think about maybe adding a locked state so we can't change state until damaged is done.

        // Reset timer if hit again
        if (damageResetCoroutine != null) StopCoroutine(damageResetCoroutine);
        damageResetCoroutine = StartCoroutine(WaitThenRevertFromDamaged());
    }
    private IEnumerator WaitThenRevertFromDamaged()
    {
        yield return new WaitForSeconds(damageStateDuration);
        if (State == State.DAMAGED) // Only revert if still in Damaged state
        {
            ReqStateChange(State.IDLE);
        }
    }

    private void SetState(State state)
    {
        State oldState = this.State;
        this.State = state;
        OnStateChanged.Invoke();
        leh.Call(new OnStateChangeEvent { newState = state, oldState = oldState });

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
