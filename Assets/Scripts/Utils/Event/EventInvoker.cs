using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Util component that invokes a Global Event usually on load.
/// </summary>
public class EventInvoker : SerializedMonoBehaviour
{
    [Tooltip("Event to invoke")]
    [Required] public IGlobalEvent globalEvent;
    [SerializeField] private bool invokeOnLoad = true;

    [Tooltip("Time to delay before invoking after start. Useful to stall a bit so that subscriptions have time to be made.")]
    [SerializeField] private float invokeDelay = 1f;

    private void Awake()
    {
        Assert.IsNotNull(globalEvent);
    }

    private void Start()
    {
        if (invokeOnLoad)
        {
            StartCoroutine(StartInvokeEvent());
        }
    }

    private IEnumerator StartInvokeEvent()
    {
        yield return new WaitForSeconds(invokeDelay);
        InvokeEvent();
    }

    private void InvokeEvent()
    {
        var eventType = globalEvent.GetType();
        Type busType = typeof(EventBus<>).MakeGenericType(eventType);
        var registerMethod = busType.GetMethod("Call");
        registerMethod.Invoke(null, new object[] { globalEvent });
    }

}
