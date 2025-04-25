using UnityEngine;

public class NoPressTrigger : IInteractionKeyPressBehaviour
{
    public bool ValidateTrigger(KeyCode key)
    {
        return true;
    }
}
