using System;
using UnityEngine;

public class TrackedEvent
{
    private readonly string eventName;
    private event Action handlers;
    private bool logInvoke;

    public TrackedEvent(string eventName, bool logInvoke = false)
    {
        this.eventName = eventName;
        this.logInvoke = logInvoke;
    }

    public void Subscribe(Action handler)
    {
        Debug.Log($"[Event: {eventName}] Subscribed by {handler.Target}.{handler.Method.Name}");
        handlers += handler;
    }

    public void Unsubscribe(Action handler)
    {
        Debug.Log($"[Event: {eventName}] Unsubscribed by {handler.Target}.{handler.Method.Name}");
        handlers -= handler;
    }

    public void Invoke()
    {
        if (logInvoke) Debug.Log($"[Event: {eventName}] Invoked");


        if (handlers == null) return;

        foreach (var d in handlers.GetInvocationList())
        {
            if (logInvoke) Debug.Log($"[Event: {eventName}] → Calling: {d.Target}.{d.Method.Name}");
        }

        handlers.Invoke();
    }
}
