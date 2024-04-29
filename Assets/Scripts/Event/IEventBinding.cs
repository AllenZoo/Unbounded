using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventBinding<T>
{
    public Action<T> OnEvent { get; set; }
    public Action OnEventNoArgs { get; set; }
}

/// <summary>
/// EventBinding reprsents a binding between an event (IEvent) and a listener (Action).
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventBinding<T> : IEventBinding<T> where T : IEvent
{
    public Action<T> onEvent = delegate(T t) { };
    public Action onEventNoArgs = delegate { };

    public Action<T> OnEvent
    {
        get => onEvent;
        set => onEvent = value;
    }

    public Action OnEventNoArgs
    {
        get => onEventNoArgs;
        set => onEventNoArgs = value;
    }

    public EventBinding(Action<T> _event)
    {
        onEvent = _event;
    }

    public EventBinding(Action _event)
    {
        onEventNoArgs = _event;
    }
}
