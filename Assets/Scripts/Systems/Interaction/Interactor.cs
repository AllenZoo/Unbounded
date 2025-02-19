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

    private void Awake()
    {
        isActive = true;
    }

    // Kees track of all interactable objects within range.
    private List<WorldInteractableObject> interactables;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        WorldInteractableObject interactable = collision.GetComponentInParent<WorldInteractableObject>();
        if (interactable == null)
        {
            return;
        }

        // Check if interactable is triggerable just by walking over/near it.
        if (activeInteractable == null)
        {
            activeInteractable = interactable;
            Debug.Log("active Interactable after being set is null? " + activeInteractable == null);
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

        if (activeInteractable.Equals(interactable))
        {
            activeInteractable = GetNextActiveInteractable();
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

    private void HandleActiveInteractableKeyPress()
    {
        if (activeInteractable == null || isActivelyInteracting) { return; }
        Debug.Log("Actively listening for input!");

        activeInteractable.DisplayPrompt();
        KeyCode reqKey = activeInteractable.RequiredKeyPress;

        // If no reqKey, object is interactable
        if (reqKey == KeyCode.None) {
            activeInteractable.Interact();
            isActivelyInteracting = true;

        } else
        {
            if (Input.GetKeyDown(reqKey))
            {
                activeInteractable.Interact();
                isActivelyInteracting = true;
            }
        }
    }

    private void Update()
    {
        HandleActiveInteractableKeyPress();
    }

}
