using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalEventConditionData", menuName = "System/Objective/Conditions/GlobalEventConditionData", order = 1)]
public class GlobalEventConditionData : ObjectiveConditionData
{
    [TypeFilter(nameof(GetEventTypes))]
    public Type globalEventType;

    public override IObjectiveCondition CreateInstance()
    {
        if (globalEventType == null || !typeof(IGlobalEvent).IsAssignableFrom(globalEventType))
        {
            Debug.LogError($"Invalid event type assigned: {globalEventType}");
            return null;
        }
        return new GlobalEventCondition(globalEventType);
    }

    private static IEnumerable<Type> GetEventTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IGlobalEvent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
    }
}

public class GlobalEventCondition : IObjectiveCondition
{
    public event Action OnStateChanged;
    private Objective owner;
    private bool eventTriggered;
    private Type eventType;
    private IEventBinding binding;

    public GlobalEventCondition(Type eventType) => this.eventType = eventType;

    public bool IsMet() => eventTriggered;

    public void Initialize(Objective owner)
    {
        this.owner = owner;
        var bindingType = typeof(EventBinding<>).MakeGenericType(eventType);
        binding = (IEventBinding)Activator.CreateInstance(bindingType);
        
        // Find the Property specifically to avoid issues
        var prop = binding.GetType().GetProperty("OnEventNoArgs");
        prop?.SetValue(binding, (Action)OnGlobalEventInvoked);
        
        EventBusUtil.Subscribe(eventType, binding);
    }

    private void OnGlobalEventInvoked()
    {
        if (owner.State != ObjectiveState.ACTIVE) return;
        eventTriggered = true;
        OnStateChanged?.Invoke();
    }

    public void Cleanup()
    {
        if (binding != null) EventBusUtil.Unsubscribe(eventType, binding);
    }
}