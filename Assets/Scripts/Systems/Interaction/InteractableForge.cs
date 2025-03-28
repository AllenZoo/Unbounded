using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO: rename this.
public class InteractableForge : WorldInteractableObject
{
    [Required]
    [SerializeField]
    private InteractablePromptData displayMessage;

    [Required]
    [SerializeField]
    [Tooltip("Reference to page that will be toggled on and off by interacting with forge.")]
    private PageUIContext pageUIContext;

    private void Awake()
    {
        Assert.IsNotNull(pageUIContext);

        // Make default display prompt = true
        InteractablePromptData newPrompt = new InteractablePromptData(displayMessage.message, displayMessage.reqKey, true);
        messageDisplayBehaviour = new MessageDisplay(soPromptData, newPrompt);
    }

    public override void Interact()
    {
        // Note: if interaction doesn't do anything, check that the relevant PageUI references the same PageUIContext SO on this object!
        Debug.Log("Interacting with Forge!");
        pageUIContext.PageUI?.MoveToTopOrClose();
    }

    public override void UnInteract()
    {
        Debug.Log("Uninteracting with Forge!");
        pageUIContext.PageUI?.ClosePage();
    }
}
