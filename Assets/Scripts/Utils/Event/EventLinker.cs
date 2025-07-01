using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

/// <summary>
/// Used to link events to function calls via inspector.
/// </summary>
public class EventLinker : SerializedMonoBehaviour
{
    [Tooltip("Hacky way to get type of event. Don't need to fill in the instance.")]
    [Required] public IGlobalEvent globalEventType;
    public UnityEvent OnEventTriggered;

    private object eventBinding;

    private void Awake()
    {
        Assert.IsNotNull(globalEventType);
    }

    private void Start()
    {
        Type eventType = globalEventType.GetType();

        Type bindingType = typeof(EventBinding<>).MakeGenericType(eventType);
        eventBinding = Activator.CreateInstance(bindingType, (Action)Trigger);

        Type busType = typeof(EventBus<>).MakeGenericType(eventType);
        var registerMethod = busType.GetMethod("Register");
        registerMethod.Invoke(null, new object[] { eventBinding });
    }

    private void OnDisable()
    {
        // TODO: check if we need to unsubscribe. I think it's unnecessary since the UTIl class handles cleanup, and doing it here
        //       also causes some Collection Modified error.
        //Type eventType = globalEventType.GetType();
        //if (eventBinding != null && globalEventType != null)
        //{
        //    Type busType = typeof(EventBus<>).MakeGenericType(eventType);
        //    var unregisterMethod = busType.GetMethod("Unregister");
        //    unregisterMethod.Invoke(null, new object[] { eventBinding });
        //}
    }

    private void OnDestroy()
    {
        Type eventType = globalEventType.GetType();
        if (eventBinding != null && globalEventType != null)
        {
            Type busType = typeof(EventBus<>).MakeGenericType(eventType);
            var unregisterMethod = busType.GetMethod("Unregister");
            unregisterMethod.Invoke(null, new object[] { eventBinding });
        }
    }


    public void Trigger()
    {
        OnEventTriggered?.Invoke();
    }

    
}
