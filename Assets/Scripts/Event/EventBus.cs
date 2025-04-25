using System.Collections.Generic;
using UnityEngine;

public static class EventBus<T>
{
    // Stores the event bindings.
    static readonly HashSet<IEventBinding<T>> eventBindings = new HashSet<IEventBinding<T>>();

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

        foreach (var eventBinding in eventBindings)
        {
            eventBinding.OnEvent?.Invoke(@event);
            eventBinding.OnEventNoArgs?.Invoke();
        }
    }

}
