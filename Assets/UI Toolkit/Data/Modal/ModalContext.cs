using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// Context = essentially singleton ScriptableObject that holds state information for a specific system.
/// </summary>
[CreateAssetMenu(fileName = "ModalContext", menuName = "System/Contexts/Modal Context", order = 1)]
public class ModalContext : ScriptableObject
{
    [ShowInInspector]
    public ModalData Payload { get; private set; }

    [ShowInInspector]
    public bool IsOpen { get; private set; }

    public Action OnChanged;


    private ICommittableInteraction interaction;


    public void Open(ModalData payload, ICommittableInteraction interaction = null)
    {
        IsOpen = true;
        Payload = payload;
        this.interaction = interaction;
        OnChanged?.Invoke();
    }

    public void Close()
    {
        IsOpen = false;
        Payload = null;
        OnChanged?.Invoke();
    }

    public void Resolve(bool confirmed)
    {
        if (!IsOpen)
            return;

        if (confirmed)
            interaction?.Commit();
        else
            interaction?.Cancel();

        interaction = null;
        Close();
    }
}
