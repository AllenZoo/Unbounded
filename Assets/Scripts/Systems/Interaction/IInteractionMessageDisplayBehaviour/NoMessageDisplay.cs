using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMessageDisplay : IInteractionMessageDisplayBehaviour
{
    public void DisplayPrompt(InteractablePromptData prompt)
    {
        return;
    }
}
