using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Interactor Class that Interacts with Interactable Objects
/// </summary>
public class Interactor : MonoBehaviour
{
    private bool isActive;

    // Keeps track of whether there is a interactable that Interactor is interacting with.
    // This prevents the interaction by being overwritten by another by accident.
    // For now, this is true and bound to first interactable object the Interactor collides with.
    private bool isActivelyInteracting;
    private WorldInteractableObject activeInteractable;

    private void Awake()
    {
        isActive = true;
    }

    // Kees track of all interactable objects within range.
    private List<WorldInteractableObject> interactables;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        WorldInteractableObject interactable = collision.GetComponent<WorldInteractableObject>();
        if (interactable == null)
        {
            return;
        }

        // Check if interactable is triggerable just by walking over/near it.
        bool triggered = interactable.ValidateTrigger(KeyCode.None);
        if (triggered)
        {
            activeInteractable = interactable;
        }

        interactables.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        WorldInteractableObject interactable = collision.GetComponent<WorldInteractableObject>();
        if (interactable == null)
        {
            return;
        }

        interactables.Remove(interactable);
    }

    /// <summary>
    /// Sorts the list by priority of interactable objects
    /// </summary>
    private void SortListByPriority()
    {
        //interactables.Sort()
    }

    private void HandleActiveInteractable()
    {
        if (activeInteractable != null)
        {
            activeInteractable.Interact();

            if (Input.GetKeyDown(KeyCode.F))
            {

            }
        }
    }

}
