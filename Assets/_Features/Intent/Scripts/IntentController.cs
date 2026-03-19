using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// Handles passing input events to appropriate systems.
/// Should solely be in charge of processing and forwarding input.
/// </summary>
public class IntentController : MonoBehaviour
{
    public LocalEventHandler LocalEventHandler { get { return leh; } private set { } }
    [NotNull]
    [SerializeField] protected LocalEventHandler leh;

    public bool inputEnabled = true;

    protected void Awake()
    {
        leh = InitializerUtil.FindComponentInParent<LocalEventHandler>(gameObject);
    }

    protected void Start()
    {
        LocalEventBinding<OnStateChangeEvent> stateChangeBinding = new LocalEventBinding<OnStateChangeEvent>(HandleStateChanged);
        leh.Register(stateChangeBinding);
    }

    public void InvokeMovementInput(Vector2 movementInput)
    {
        if (!inputEnabled)
        {
            return;
        }
        leh.Call(new OnMovementInput { movementInput = movementInput });
    }

    public void InvokeAttackInput(KeyCode keyCode, AttackSpawnInfo attackInfo)
    {
        if (!inputEnabled)
        {
            return;
        }
        leh.Call(new OnAttackInput { keyCode = keyCode, attackInfo = attackInfo });
    }

    /// <summary>
    /// TODO: eventually refactor thisi such that the input controller doesn't have to know about states, and instead just listens for events that disable/enable input. 
    /// This would allow for more modularity and separation of concerns.
    /// 
    /// (e.g. instead of having this function, set inputEnabled with a Setter.)
    /// </summary>
    /// <param name="e"></param>
    /// Input Controller Should not know that we have to disable input when stunned or dead, but for now this is the simplest way to implement this functionality. (gameplay logic)
    private void HandleStateChanged(OnStateChangeEvent e)
    {
        // Disable input when stunned or dead.
        switch (e.newState)
        {
            // fall through
            case State.STUNNED:
            case State.DEAD:
                inputEnabled = false;
                break;
            default:
                inputEnabled = true;
                break;
        }
    }

}
