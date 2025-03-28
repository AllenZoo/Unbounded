using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: rename this.
public class InteractableForge : WorldInteractableObject
{
    [Required]
    [SerializeField]
    private InteractablePromptData displayMessage;

    [Required]
    [SerializeField]
    [Tooltip("Reference to page that will be toggled on and off by interacting with forge.")]
    // TODO: make this a scriptable object reference.
    private PageUI pageUI;

    private void Awake()
    {
        // Make default display prompt = true
        InteractablePromptData newPrompt = new InteractablePromptData(displayMessage.message, displayMessage.reqKey, true);
        messageDisplayBehaviour = new MessageDisplay(soPromptData, newPrompt);
    }

    public override void Interact()
    {
        Debug.Log("Interacting with Forge!");
        pageUI.MoveToTopOrClose();
    }

    public override void UnInteract()
    {
        Debug.Log("Uninteracting with Forge!");
        pageUI.ClosePage();
    }
}
