using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to components that act as objective points. Function will be called from UnityEvent.
/// </summary>
public class ObjectivePoint : MonoBehaviour
{
    public Action<Objective> OnObjectivePointComplete;

    private Objective objective;

    public void CompleteObjective()
    {
        if (objective == null) {
            Debug.LogError("Tried to complete objective point that isn't owned by any objective.");
            return;
        }

        OnObjectivePointComplete?.Invoke(objective);
    }

    public void SetObjective(Objective obj)
    {
        this.objective = obj;
    }
}
