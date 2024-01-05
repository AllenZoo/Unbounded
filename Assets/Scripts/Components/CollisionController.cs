using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Currently handles collision between Player and interactables
// TODO: rename?
public class CollisionController : MonoBehaviour
{
    private List<IInteractable> triggeredInteractables = new List<IInteractable>();

    // List of interactables waiting for KeyPress.
    private List<IInteractable> untriggerdInteractables = new List<IInteractable> ();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if collided object has an IInteractable component
        IInteractable interactable = collision.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // TODO: account for priority before adding to list.
            // High priority interactables should be added to the back of the list. ie. interacted with first.
            
            // TODO: account for RequiresKeyPress. If true, then only add to list if key is pressed.

            triggeredInteractables.Add(interactable);
            InteractWithMostRecentTrigger();
        }
        // Debug.Log(gameObject.name + " triggered with " + collision.gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.gameObject.GetComponent<IInteractable>();
        if (interactable != null)
        {
            // If last interactable in list, then uninteract.
            if (triggeredInteractables.Count <= 1)
            {
                interactable.UnInteract();
            }

            triggeredInteractables.Remove(interactable);
            InteractWithMostRecentTrigger();
        }
    }

    private void InteractWithMostRecentTrigger()
    {
        if (triggeredInteractables.Count > 0)
        {
            IInteractable mostRecentTrigger = triggeredInteractables[triggeredInteractables.Count - 1];
            mostRecentTrigger.Interact();
        }
    }
}
