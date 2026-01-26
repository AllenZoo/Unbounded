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

    [OdinSerialize]
    private List<UIContextTrigger> triggers;



    private void Awake()
    {
        Assert.IsNotNull(pageUIContext);

        // Make default display prompt = true
        InteractablePromptData newPrompt = new InteractablePromptData(displayMessage, true);
        messageDisplayBehaviour = new MessageDisplay(soPromptData, newPrompt);
    }

    public override void Interact()
    {
        // Interact triggers when Interactor and Key Press conditions pass.

        // Note: if interaction doesn't do anything, check that the relevant PageUI references the same PageUIContext SO on this object!
        if (Debug.isDebugBuild)
        {
            Debug.Log("Interacting with Prompter!");
        }

        foreach (var trigger in triggers)
        {
            if (trigger.context is IPayloadedUIContext payloaded)
                payloaded.Open(trigger.payload);
            else
                trigger.context.Open();
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
        
        pageUIContext.PageUI?.ClosePage();
        OnUninteract?.Invoke();
    }
}

[System.Serializable]
public class UIContextTrigger
{
    public UIContext context;
    public UIContextPayload payload;
}
