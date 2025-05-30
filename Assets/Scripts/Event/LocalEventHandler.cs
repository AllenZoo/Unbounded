using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalEventHandler : MonoBehaviour
{
    [SerializeField] private LocalEventHandlerContext context;
    private Dictionary<Type, object> eventBuses = new Dictionary<Type, object>();

    public void Start()
    {
        context?.Init(this);
    }

    public void Register<T>(LocalEventBinding<T> eventBinding) where T : ILocalEvent
    {
        Type eventType = typeof(T);
        if (!eventBuses.ContainsKey(eventType))
        {
            eventBuses[eventType] = new LocalEventBus<T>();
        }
        ((LocalEventBus<T>)eventBuses[eventType]).Register(eventBinding);
    }

    public void Unregister<T>(LocalEventBinding<T> eventBinding) where T : ILocalEvent
    {
        Type eventType = typeof(T);
        if (eventBuses.ContainsKey(eventType))
        {
            ((LocalEventBus<T>)eventBuses[eventType]).Unregister(eventBinding);
        }
    }

    public void Call(ILocalEvent localEvent)
    {
        Type eventType = localEvent.GetType();
        if (eventBuses.ContainsKey(eventType))
        {
            eventBuses[eventType].GetType().GetMethod("Call").Invoke(eventBuses[eventType], new object[] { localEvent });
        }
    }

    public void UnregisterAll()
    {
        foreach (var bus in eventBuses.Values)
        {
            var method = bus.GetType().GetMethod("UnregisterAll");
            method?.Invoke(bus, null);
        }

        eventBuses.Clear(); // optional: clears the buses dictionary
    }

    private void OnDestroy()
    {
        UnregisterAll();
    }


}
