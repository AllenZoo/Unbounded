using System;
using UnityEngine;

/// <summary>
/// Context = essentially singleton ScriptableObject that holds state information for a specific system.
/// </summary>
[CreateAssetMenu(fileName = "ModalContext", menuName = "System/Contexts/Modal Context", order = 1)]
public class ModalContext : ScriptableObject
{
    public ModalData Payload { get; private set; }
    public bool IsOpen { get; private set; }

    public Action OnChanged;

    public void Open(ModalData payload = null)
    {
        IsOpen = true;
        Payload = payload;
        OnChanged?.Invoke();
    }

    public void Close()
    {
        IsOpen = false;
        Payload = null;
        OnChanged?.Invoke();
    }
}
