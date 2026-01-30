using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class EventBus<T>
{
    // Stores the event bindings.
    static readonly HashSet<IEventBinding<T>> eventBindings = new HashSet<IEventBinding<T>>();

    static EventBus()
    {
        EventBusRegistry.Register(eventBindings.Clear);
    }

    static void Reset()
    {
        Debug.Log($"[EventBus<{typeof(T)}>] Resetting event bindings.");
        eventBindings.Clear();
    }

    public static void Register(IEventBinding<T> eventBinding)
    {
        eventBindings.Add(eventBinding);
    }

    public static void Unregister(IEventBinding<T> eventBinding)
    {
        eventBindings.Remove(eventBinding);
    }

    public static void Call(T @event)
    {
        if (Debug.isDebugBuild)
            Debug.Log($"[EventBus<{typeof(T)}>] Fired: {@event}. Listeners: {eventBindings.Count}");

        // Snapshot to avoid modification during iteration
        var snapshot = eventBindings.ToArray();

        foreach (var eventBinding in snapshot)
        {
            eventBinding.OnEvent?.Invoke(@event);
            eventBinding.OnEventNoArgs?.Invoke();
        }
    }
}
