using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;

public abstract class WorldInteractableObject : SerializedMonoBehaviour, IInteractableObject
{
    [FoldoutGroup("Events")]
    public UnityEvent OnInteract;

    [FoldoutGroup("Events")]
    public UnityEvent OnUninteract;


    // Priority
    public float Priority
    {
        get { return priority; }
        private set { priority = value; }
    }
    [SerializeField, Tooltip("Priority of the interactable in case of overlap. Higher priority interactables get displayed first!")]
    protected float priority = 5;

    // Required Key Press
    public KeyCode RequiredKeyPress { get { return requiredKeyPress; } }
    [SerializeField]
    protected KeyCode requiredKeyPress = KeyCode.None;

    // Prompt Data
    [Required, SerializeField]
    protected SO_InteractablePromptData soPromptData;


    protected IInteractionMessageDisplayBehaviour messageDisplayBehaviour = new NoMessageDisplay();

    public abstract void Interact();
    public abstract void UnInteract();

    /// <summary>
    /// Displays Prompt via filling the shared scriptable object with data.
    /// If it doesn't work, maybe check if UI object has correct reference to SO?
    /// </summary>
    /// <param name="prompt"></param>
    public void DisplayPrompt()
    {
        messageDisplayBehaviour.DisplayPrompt();
    }

    public void HidePrompt()
    {
        messageDisplayBehaviour.HidePrompt();
    }
}
