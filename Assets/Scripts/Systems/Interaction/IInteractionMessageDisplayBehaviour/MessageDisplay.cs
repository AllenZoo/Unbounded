using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageDisplay : IInteractionMessageDisplayBehaviour
{
    // The reference we use to set the prompt.
    private SO_InteractablePromptData interactablePromptData { get; set; }

    public MessageDisplay(SO_InteractablePromptData interactablePromptData)
    {
        this.interactablePromptData = interactablePromptData;
    }

    public void DisplayPrompt(InteractablePromptData prompt)
    {
        interactablePromptData.SetData(prompt);
    }
}
