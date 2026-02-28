using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoPressTrigger : IInteractionKeyPressBehaviour
{
    public bool ValidateTrigger(KeyCode key)
    {
        return true;
    }
}
