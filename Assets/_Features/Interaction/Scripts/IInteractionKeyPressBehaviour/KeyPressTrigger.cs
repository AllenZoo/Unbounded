using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPressTrigger : IInteractionKeyPressBehaviour
{
    private KeyCode requiredKey;

    public KeyPressTrigger(KeyCode requiredKey)
    {
        this.requiredKey = requiredKey;
    }

    public bool ValidateTrigger(KeyCode key)
    {
        return requiredKey == key;
    }
}
