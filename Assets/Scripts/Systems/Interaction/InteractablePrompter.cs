using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class InteractablePrompter : WorldInteractableObject
{

    [SerializeField] private string displayMessage = "";
    [SerializeField] private UnityEvent OnInteract;
    [SerializeField] private UnityEvent OnUninteract;

    [Required, SerializeField]
    [Tooltip("Reference to page that will be toggled on and off by interacting with forge.")]
    private PageUIContext pageUIContext;

    private void Awake()
    {
        Assert.IsNotNull(pageUIContext);

        // Make default display prompt = true
        InteractablePromptData newPrompt = new InteractablePromptData(displayMessage, requiredKeyPress, true);
        messageDisplayBehaviour = new MessageDisplay(soPromptData, newPrompt);
    }

    public override void Interact()
    {
        // Note: if interaction doesn't do anything, check that the relevant PageUI references the same PageUIContext SO on this object!
        if (Debug.isDebugBuild)
        {
            Debug.Log("Interacting with Prompter!");
        }
        
        pageUIContext.PageUI?.MoveToTopOrClose();
        OnInteract?.Invoke();
    }

    public override void UnInteract()
    {
        if (Debug.isDebugBuild)
        {
            Debug.Log("Uninteracting with Prompter!");
        }
            
        pageUIContext.PageUI?.ClosePage();
        OnUninteract?.Invoke();
    }
}
