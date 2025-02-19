using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class WorldInteractableObject : MonoBehaviour, IInteractableObject
{

    public float Priority
    {
        get { return priority; }
        private set { priority = value; }
    }

    public KeyCode RequiredKeyPress { get { return requiredKeyPress; } }

    [Required]
    [SerializeField]
    protected SO_InteractablePromptData soPromptData;

    [SerializeField]
    protected float priority = 5;

    [SerializeField]
    protected KeyCode requiredKeyPress = KeyCode.None;

    protected IInteractionMessageDisplayBehaviour messageDisplayBehaviour = new NoMessageDisplay();

    public abstract void Interact();
    public abstract void UnInteract();


    /// <summary>
    /// Displays Prompt via filling the shared scriptable object with data.
    /// If it doesn't work, maybe check if UI object has correct reference to SO?
    /// </summary>
    /// <param name="prompt"></param>
    public abstract void DisplayPrompt();
}
