using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageDisplay : IInteractionMessageDisplayBehaviour
{
    // The reference we use to set the prompt.
    private SO_InteractablePromptData soPromptDataRef { get; set; }
    private InteractablePromptData promptData;

    public MessageDisplay(SO_InteractablePromptData soPromptDataRef, InteractablePromptData promptData)
    {
        this.soPromptDataRef = soPromptDataRef;
        this.promptData = promptData;
    }

    public void DisplayPrompt()
    {
        soPromptDataRef.SetData(promptData);
    }
}
