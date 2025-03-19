using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

/// <summary>
/// Handles passing input events to appropriate systems.
/// </summary>
public class InputController : MonoBehaviour
{
    public LocalEventHandler LocalEventHandler { get { return localEventHandler; } private set { } }
    [NotNull]
    [SerializeField] protected LocalEventHandler localEventHandler;

    public bool inputEnabled = true;

    protected void Awake()
    {
        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                    "] with root object [" + gameObject.transform.root.name + "] for InputController.cs");
            }
        }
    }

    protected void Start()
    {
        LocalEventBinding<OnStateChangeEvent> stateChangeBinding = new LocalEventBinding<OnStateChangeEvent>(HandleStateChanged);
        localEventHandler.Register(stateChangeBinding);
    }

    public void InvokeMovementInput(Vector2 movementInput)
    {
        if (!inputEnabled)
        {
            return;
        }
        localEventHandler.Call(new OnMovementInput { movementInput = movementInput });
    }

    public void InvokeAttackInput(KeyCode keyCode, AttackSpawnInfo attackInfo)
    {
        if (!inputEnabled)
        {
            return;
        }
        localEventHandler.Call(new OnAttackInput { keyCode = keyCode, attackInfo = attackInfo });
    }

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
