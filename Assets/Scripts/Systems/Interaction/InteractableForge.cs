using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableForge : WorldInteractableObject
{
    [Required]
    [SerializeField]
    private InteractablePromptData displayMessage;


    private void Awake()
    {
        messageDisplayBehaviour = new MessageDisplay(soPromptData, displayMessage);
    }

    public override void Interact()
    {
        // TODO: open forgeUI
        Debug.Log("Opening Forge UI!");
        throw new System.NotImplementedException();
    }

    public override void UnInteract()
    {
        // TODO: close forgeUI
        Debug.Log("Closing Forge UI!");
        throw new System.NotImplementedException();
    }

    public override void DisplayPrompt()
    {
        messageDisplayBehaviour.DisplayPrompt();
    }
}
