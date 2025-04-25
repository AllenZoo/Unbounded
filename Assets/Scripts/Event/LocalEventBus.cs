using System.Collections.Generic;

public class LocalEventBus<T> where T : ILocalEvent
{
    // Stores the event bindings.
    readonly HashSet<LocalEventBinding<T>> eventBindings = new HashSet<LocalEventBinding<T>>();
    public void Register(LocalEventBinding<T> eventBinding)
    {
        eventBindings.Add(eventBinding);
    }
    public void Unregister(LocalEventBinding<T> eventBinding)
    {
        eventBindings.Remove(eventBinding);
    }
    public void Call(T @event)
    {
        foreach (var eventBinding in eventBindings)
        {
            eventBinding.OnEvent?.Invoke(@event);
            eventBinding.OnEventNoArgs?.Invoke();
        }

        /* Unmerged change from project 'Assembly-CSharp.Player'
        Before:
            }

            public void Clear()
        After:
            }

            public void Clear()
        */
    }

    public void Clear()
    {
        eventBindings.Clear();
    }

    public void UnregisterAll()
    {
        eventBindings.Clear();
    }
}
