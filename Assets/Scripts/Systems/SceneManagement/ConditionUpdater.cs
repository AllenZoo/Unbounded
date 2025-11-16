using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// A manager level class that updates conditions that change after an event is called.
/// 
/// For example, if OnStarterWeaponCardEffectApply is called/invoked, we can make it set PlayerStarterWeaponEquipped SerializableObjectBoolean state to true.
/// </summary>
public class ConditionUpdater : SerializedMonoBehaviour
{
    [SerializeField] private List<ConditionEventBinding> eventBindings = new();

    private void OnEnable()
    {
        foreach (var binding in eventBindings)
        {
            ConditionEventBinder.Register(binding);
        }
    }

    private void OnDisable()
    {
        foreach (var binding in eventBindings)
        {
            ConditionEventBinder.Unregister(binding);
        }
    }

    private void OnDestroy()
    {
        foreach (var binding in eventBindings)
        {
            ConditionEventBinder.Unregister(binding);
        }
    }

    // TODO; figure out a way so that we can link event calls via inspector.
    public void UpdateCondition(ConditionEventBinding binding)
    {

    }
}

[Serializable]
public class ConditionEventBinding
{
    [Tooltip("Event to subscribe to.")]
    [SerializeField, SerializeReference, AssetsOnly]
    public IGlobalEvent eventType;

    [Tooltip("Condition to set when the event fires.")]
    [InlineEditor, HideLabel]
    public ScriptableObjectBoolean condition;

    [Tooltip("Value to apply to the condition.")]
    public bool newValue;

    [NonSerialized] public object runtimeBinding; // Store actual IEventBinding<T>
}

public static class ConditionEventBinder
{
    public static void Register(ConditionEventBinding binding)
    {
        if (binding.eventType == null || binding.condition == null)
            return;

        // Create a generic EventBinding<T>
        Type genericBindingType = typeof(EventBinding<>).MakeGenericType(binding.eventType.GetType());
        var bindingInstance = Activator.CreateInstance(genericBindingType);

        // Set OnEventNoArgs = () => binding.condition.Set(binding.newValue)
        var onEventNoArgsField = genericBindingType.GetField("onEventNoArgs");
        Action callback = () => binding.condition.Set(binding.newValue);
        onEventNoArgsField.SetValue(bindingInstance, callback);

        // Store reference for unregistering later
        binding.runtimeBinding = bindingInstance;

        // Register with EventBus<T>
        MethodInfo registerMethod = typeof(EventBus<>)
            .MakeGenericType(binding.eventType.GetType())
            .GetMethod("Register");

        registerMethod.Invoke(null, new object[] { bindingInstance });
    }

    public static void Unregister(ConditionEventBinding binding)
    {
        if (binding.runtimeBinding == null || binding.eventType == null)
            return;

        MethodInfo unregisterMethod = typeof(EventBus<>)
            .MakeGenericType(binding.eventType.GetType())
            .GetMethod("Unregister");

        unregisterMethod.Invoke(null, new object[] { binding.runtimeBinding });
    }
}
