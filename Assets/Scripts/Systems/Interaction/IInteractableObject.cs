 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableObject
{
    void Interact();
    void UnInteract();
    void DisplayPrompt();
    // think about maybe
    // void Interact(KeyCode. key) or like void Interact(InteractTrigger t)
}
