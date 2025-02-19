using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class WorldInteractableObject : MonoBehaviour, IInteractableObject, IInteractionKeyPressBehaviour, IInteractionMessageDisplayBehaviour
{

    public float Priority
    {
        get { return priority; }
        private set { priority = value; }
    }

    [Required]
    [SerializeField]
    protected SO_InteractablePromptData promptData;

    [SerializeField]
    protected float priority;

    // Default behaviours
    protected IInteractionKeyPressBehaviour interactionKeyPressBehaviour = new NoPressTrigger();
    protected IInteractionMessageDisplayBehaviour messagedisplayBehaviour = new NoMessageDisplay();

    public abstract void Interact();
    public abstract void UnInteract();

    /// <summary>
    /// Validation Trigger
    /// </summary>
    /// <param name="keycode"></param>
    /// <returns></returns>
    public bool ValidateTrigger(KeyCode keycode)
    {
        return interactionKeyPressBehaviour.ValidateTrigger(keycode);
    }

    /// <summary>
    /// Displays Prompt via filling the shared scriptable object with data.
    /// If it doesn't work, maybe check if UI object has correct reference to SO?
    /// </summary>
    /// <param name="prompt"></param>
    public void DisplayPrompt(InteractablePromptData prompt)
    {
        messagedisplayBehaviour.DisplayPrompt(prompt); 
    }
}
