
/* Unmerged change from project 'Assembly-CSharp.Player'
Before:
 using System.Collections;
After:
using System.Collections;
*/
public interface IInteractableObject
{
    void Interact();
    void UnInteract();
    void DisplayPrompt();
    // think about maybe
    // void Interact(KeyCode. key) or like void Interact(InteractTrigger t)
}
