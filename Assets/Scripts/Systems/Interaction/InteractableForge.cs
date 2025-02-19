using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableForge : WorldInteractableObject
{

    private void Awake()
    {
        interactionKeyPressBehaviour = new KeyPressTrigger(KeyCode.F);
        messagedisplayBehaviour = new MessageDisplay(promptData);
    }

    public override void Interact()
    {
        // TODO: open forgeUI
        Debug.Log("Opening Forge UI!");
        throw new System.NotImplementedException();
    }

    public override void UnInteract()
    {
        // TODO: close forgeUI
        Debug.Log("Closing Forge UI!");
        throw new System.NotImplementedException();
    }
}
