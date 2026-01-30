using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBusRegistry
{
    static readonly List<Action> resetters = new();

    public static void Register(Action reset)
    {
        resetters.Add(reset);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetAll()
    {
        Debug.Log("[EventBusRegistry] Resetting all event buses.");
        foreach (var r in resetters)
            r();
    }
}
