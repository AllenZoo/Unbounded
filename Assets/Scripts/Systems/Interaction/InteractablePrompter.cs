using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

/// <summary>
/// Interactable Prompter
/// 
/// To use: 
///   1. Attach to an object.
///   2. Make sure the object/child of said object contains a Interactable Collider (layer of collider needs to be set to InteractCollider).
///      Also make sure to set IsTrigger.
///     
/// Extra:
///     * If pageUI not neccessary, there is a 'EmptyPage' reference to fill in the field.
/// </summary>
public class InteractablePrompter : WorldInteractableObject
{
    [SerializeField, TextArea(5, 8)] private string displayMessage = "";

    [Required, SerializeField]
    [Tooltip("Reference to page that will be toggled on and off by interacting with prompter. eg. ForgePage. If none, set to Empty Page.")]
    private PageUIContext pageUIContext;

    [SerializeField, Tooltip("Commands to execute on interact/uninteract.")]
    private List<UITriggerCommand> commands;


    private void Awake()
    {
        Assert.IsNotNull(pageUIContext);

        // Make default display prompt = true
        InteractablePromptData newPrompt = new InteractablePromptData(displayMessage, true);
        messageDisplayBehaviour = new MessageDisplay(soPromptData, newPrompt);
    }

    protected virtual void OnEnable()
    {
        foreach (var precondition in preconditions)
        {
            if (precondition is BooleanPreconditionData boolCondition)
            {
                boolCondition.Condition.OnValueChanged += RefreshPrompt;
            }
        }
    }

    protected virtual void OnDisable()
    {
        foreach (var precondition in preconditions)
        {
            if (precondition is BooleanPreconditionData boolCondition)
            {
                boolCondition.Condition.OnValueChanged -= RefreshPrompt;
            }
        }
    }

    public override bool CanInteract(out string failureMessage)
    {
        bool res = base.CanInteract(out failureMessage);

        if (res)
        {
            // Conditions met, reset display message to normal.
            failureMessage = displayMessage;
        }

        return res;
    }
    public override void Interact()
    {
        // Redundent Check, but just in case. (We already check in Interactor.cs OnTriggerEnter2D)
        if (!CanInteract(out string failureMessage))
        {
            
            DisplayPrompt(failureMessage);
            return;
        } else
        {
            DisplayPrompt(displayMessage);
        }

        foreach (var command in commands)
        {
            command.Execute();
        }

        pageUIContext.PageUI?.MoveToTopOrClose();
        OnInteract?.Invoke();
    }
    public override void UnInteract()
    {
        // UnInteract should only be triggered when Interactor leaves Interactable collider.
        if (Debug.isDebugBuild)
        {
            Debug.Log("Uninteracting with Prompter!");
        }

        foreach (var command in commands)
        {
            command.Undo();
        }

        pageUIContext.PageUI?.ClosePage();
        OnUninteract?.Invoke();
    }

    private void RefreshPrompt()
    {
        if (CanInteract(out string failureMessage))
        {
            DisplayPrompt(displayMessage);
        }
        else
        {
            DisplayPrompt(failureMessage);
        }
    }

}
