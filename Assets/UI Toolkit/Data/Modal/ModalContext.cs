using NUnit.Framework;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
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


    private List<ICommittableInteraction> interactions;

    public void Open(ModalData payload, List<ICommittableInteraction> interactions = null)
    {
        // Only register if not already open
        bool wasOpen = IsOpen;
        
        IsOpen = true;
        Payload = payload;
        this.interactions = interactions;
        OnChanged?.Invoke();
        
        // Register UI state change with UIStateManager only if newly opened
        if (!wasOpen && UIStateManager.Instance != null)
        {
            UIStateManager.Instance.RegisterUIOpen();
        }
    }

    public void Open(ModalData payload, ICommittableInteraction interaction = null)
    {
        // Only register if not already open
        bool wasOpen = IsOpen;
        
        IsOpen = true;
        Payload = payload;
        this.interactions = new List<ICommittableInteraction>() { interaction };
        OnChanged?.Invoke();
        
        // Register UI state change with UIStateManager only if newly opened
        if (!wasOpen && UIStateManager.Instance != null)
        {
            UIStateManager.Instance.RegisterUIOpen();
        }
    }

    public void Open(ModalData payload)
    {
        // Only register if not already open
        bool wasOpen = IsOpen;
        
        IsOpen = true;
        Payload = payload;
        interactions = null;
        OnChanged?.Invoke();
        
        // Register UI state change with UIStateManager only if newly opened
        if (!wasOpen && UIStateManager.Instance != null)
        {
            UIStateManager.Instance.RegisterUIOpen();
        }
    }

    public void Close()
    {
        // Only unregister if it was open
        bool wasOpen = IsOpen;
        
        IsOpen = false;
        Payload = null;
        OnChanged?.Invoke();
        
        // Register UI state change with UIStateManager only if it was open
        if (wasOpen && UIStateManager.Instance != null)
        {
            UIStateManager.Instance.RegisterUIClose();
        }
    }

    public void Resolve(bool confirmed)
    {
        if (!IsOpen)
            return;


        if (interactions == null)
        {
            // Nothing to commit or cancel
        }
        else if (confirmed)
        {
            foreach (var interaction in interactions)
                interaction?.Commit();
        }
        else
        {
            foreach (var interaction in interactions)
                interaction?.Cancel();
        }

        interactions = null;
        Close();
    }
}
