using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Interactor Class that Interacts with Interactable Objects
/// </summary>
public class Interactor : MonoBehaviour
{
    // Keeps track of whether Interactor itself is active
    private bool isActive;

    // Reference to current active Interactable
    private WorldInteractableObject activeInteractable;
    private bool isActivelyInteracting;

    // Kees track of all interactable objects within range.
    private List<WorldInteractableObject> interactables;

    private void Awake()
    {
        isActive = true;
        isActivelyInteracting = false;
        activeInteractable = null;
        interactables = new List<WorldInteractableObject>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        WorldInteractableObject interactable = collision.GetComponentInParent<WorldInteractableObject>();
        if (interactable == null)
        {
            return;
        }

        // Debug.Log("Triggered interaction with: " + collision.gameObject.transform.parent.name);
        // Check if interactable is triggerable just by walking over/near it.
        if (activeInteractable == null)
        {
            activeInteractable = interactable;
            activeInteractable.DisplayPrompt();
        }

        interactables.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        WorldInteractableObject interactable = collision.GetComponentInParent<WorldInteractableObject>();
        if (interactable == null)
        {
            return;
        }

        Debug.Log("Exit Triggered interaction with: " + collision.gameObject.transform.parent.name);
        if (activeInteractable.Equals(interactable))
        {
            activeInteractable.HidePrompt();
            activeInteractable.UnInteract();
            activeInteractable = null; //GetNextActiveInteractable();
        }

        isActivelyInteracting = false;
        interactables.Remove(interactable);
    }

    /// <summary>
    /// Sorts the list by priority of interactable objects
    /// </summary>
    private void SortListByPriority()
    {
        // TODO
        //interactables.Sort()
    }

    /// <summary>
    /// Get the top of the list interactable to set as active interactable
    /// </summary>
    /// <returns></returns>
    private WorldInteractableObject GetNextActiveInteractable()
    {
        // TODO
        return null;
    }

    private void HandleInteractableKeyPress()
    {
        if (activeInteractable == null) { return; }

        KeyCode reqKey = activeInteractable.RequiredKeyPress;

        // If no reqKey, object is interactable
        if (reqKey == KeyCode.None)
        {
            if (!isActivelyInteracting)
            {
                activeInteractable.Interact();
                isActivelyInteracting = true;
            }
        }
        else
        {
            if (Input.GetKeyDown(reqKey))
            {
                // For now, pressing key will always interact with object.
                // Moving away will trigger UnInteract.
                activeInteractable.Interact();
                isActivelyInteracting = true;

                // Toggle between Interact and UnInteract based on current state
                //if (!isActivelyInteracting)
                //{
                //    activeInteractable.Interact();
                //    isActivelyInteracting = true;
                //}
                //else
                //{
                //    activeInteractable.UnInteract();
                //    isActivelyInteracting = false;
                //}
            }
        }
    }

    private void Update()
    {
        if (interactables.Count > 0)
        {
            HandleInteractableKeyPress();
        }
    }

}
