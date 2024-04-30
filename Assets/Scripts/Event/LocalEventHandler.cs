using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Acts like an event bus for local events.
/// </summary>
public class LocalEventHandler : MonoBehaviour
{
    private Dictionary<ILocalEvent, HashSet<LocalEventBinding<ILocalEvent>>> eventMapping;

    public void Register(ILocalEvent localEvent, LocalEventBinding<ILocalEvent> eventBinding)
    {
        // Check if mapping exists, if not create it.
        if (!eventMapping.ContainsKey(localEvent))
        {
            eventMapping.Add(localEvent, new HashSet<LocalEventBinding<ILocalEvent>>());
        }
        eventMapping[localEvent].Add(eventBinding);
    }

    public void Unregister(ILocalEvent localEvent, LocalEventBinding<ILocalEvent> eventBinding)
    {
        if (eventMapping.ContainsKey(localEvent))
        {
            eventMapping[localEvent].Remove(eventBinding);
        }
    }

    public void Call(ILocalEvent localEvent)
    {
        if (eventMapping.ContainsKey(localEvent))
        {
            foreach (var eventBinding in eventMapping[localEvent])
            {
                eventBinding.OnEvent?.Invoke(localEvent);
                eventBinding.OnEventNoArgs?.Invoke();
            }
        }
    }
}
