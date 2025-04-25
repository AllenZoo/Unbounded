using UnityEngine;

public interface IInteractable
{
    // Higher priority interactables means they will be interacted with first
    public float Priority { get; set; }
    public bool RequiresKeyPress { get; set; }

    // The key that needs to be pressed to interact with this interactable
    // If RequiresKeyPress is false, this value is ignored
    public KeyCode Key { get; set; }

    public bool IsInteracting { get; set; }

    public void Interact();
    public void UnInteract();
}
