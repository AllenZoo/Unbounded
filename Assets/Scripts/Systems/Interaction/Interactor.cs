using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Interactor Class that Interacts with Interactable Objects
/// </summary>
public class Interactor : MonoBehaviour
{
    // Reference to current active Interactable
    private WorldInteractableObject activeInteractable;
    private bool isActivelyInteracting;

    // Kees track of all interactable objects within range.
    private List<WorldInteractableObject> interactables;

    private void Awake()
    {
        isActivelyInteracting = false;
        activeInteractable = null;
        interactables = new List<WorldInteractableObject>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        WorldInteractableObject interactable = collision.GetComponent<WorldInteractableObject>();
        if (interactable == null)
        {
            return;
        }

        interactables.Add(interactable);
        SortListByPriority();
        activeInteractable = GetNextActiveInteractable();
        //activeInteractable.CanInteract(out var failureMessage);
        //activeInteractable.DisplayPrompt();

        if (!activeInteractable.CanInteract(out var failureMessage))
        {
            activeInteractable.DisplayPrompt(failureMessage);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        WorldInteractableObject interactable = collision.GetComponent<WorldInteractableObject>();
        if (interactable == null)
        {
            return;
        }

        if (interactable.Equals(activeInteractable))
        {
            isActivelyInteracting = false;
        }

        interactable.HidePrompt();
        interactable.UnInteract();

        interactables.Remove(interactable);

        SortListByPriority();
        activeInteractable = GetNextActiveInteractable();
    }

    /// <summary>
    /// Sorts the list by priority of interactable objects
    /// </summary>
    private void SortListByPriority()
    {
        // Sort from highest priority first to lowest priority last.
        interactables.Sort((interactable1, interactable2) =>
        {
            return (int)(interactable2.Priority - interactable1.Priority);
        });
    }

    /// <summary>
    /// Get the top of the list interactable to set as active interactable
    /// </summary>
    /// <returns></returns>
    private WorldInteractableObject GetNextActiveInteractable()
    {
        if (interactables.Count == 0) return null;
        return interactables[0];
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
            }
        }
    }
    private void CleanUpInactiveObjects()
    {
        interactables.RemoveAll(obj => obj == null || !obj.gameObject.activeInHierarchy);
        SortListByPriority();
        activeInteractable = GetNextActiveInteractable();

        if (activeInteractable != null)
        {
            activeInteractable.DisplayPrompt();
        }
    }

    private void Update()
    {
        if (interactables.Count > 0)
        {
            HandleInteractableKeyPress();
        }
        CleanUpInactiveObjects();
    }

}
