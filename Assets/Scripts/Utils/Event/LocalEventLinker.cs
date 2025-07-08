using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

/// <summary>
/// Used to link events to function calls via inspector.
/// </summary>
public class LocalEventLinker : SerializedMonoBehaviour
{
    [SerializeField, Required] private LocalEventHandler leh;

    [Tooltip("Hacky way to get type of event. Don't need to fill in the instance.")]
    [SerializeField, Required] private ILocalEvent globalEventType;
    public UnityEvent OnEventTriggered;

    private object eventBinding;

    private void Awake()
    {
        Assert.IsNotNull(leh);
        Assert.IsNotNull(globalEventType);
    }

    private void Start()
    {
        Type eventType = globalEventType.GetType();

        // Create LocalEventBinding<T> with Action
        Type bindingType = typeof(LocalEventBinding<>).MakeGenericType(eventType);
        eventBinding = Activator.CreateInstance(bindingType, (Action)Trigger);

        // Call Register<T>(LocalEventBinding<T>) on 'leh' via reflection
        var registerMethod = typeof(LocalEventHandler)
            .GetMethod("Register")
            .MakeGenericMethod(eventType);

        registerMethod.Invoke(leh, new object[] { eventBinding });
    }

    private void OnDisable()
    {
        Type eventType = globalEventType.GetType();
        if (eventBinding != null && globalEventType != null)
        {
            Type busType = typeof(EventBus<>).MakeGenericType(eventType);
            var unregisterMethod = busType.GetMethod("Unregister");
            unregisterMethod.Invoke(null, new object[] { eventBinding });
        }
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            leh.Call(new OnDeathEvent());
        }
    }

    public void Trigger()
    {
        Debug.Log("event triggered!");
        OnEventTriggered?.Invoke();
    }

}
