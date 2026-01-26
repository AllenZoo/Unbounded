using NUnit.Framework;
using System;
using UnityEngine;

/// <summary>
/// Context = essentially singleton ScriptableObject that holds state information for a specific system.
/// </summary>
[CreateAssetMenu(fileName = "ModalContext", menuName = "System/Contexts/Modal Context", order = 1)]
public class ModalContext : ScriptableObject, UIContext, IPayloadedUIContext
{
    public ModalData Payload { get; private set; }
    public bool IsOpen { get; private set; }

    public Action OnChanged;

    public void Open(UIContextPayload payload)
    {
        IsOpen = true;
        Payload = payload as ModalData;
        Assert.IsNotNull(Payload);
        OnChanged?.Invoke();
    }

    public void Open() { 
        IsOpen = true;
        OnChanged?.Invoke();
    }

    public void Close()
    {
        IsOpen = false;
        Payload = null;
        OnChanged?.Invoke();
    }
}
