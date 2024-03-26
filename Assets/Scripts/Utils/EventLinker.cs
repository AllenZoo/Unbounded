using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Used to link events to function calls via inspector.
/// </summary>
public class EventLinker : MonoBehaviour
{
    public Action trigger;
    public UnityEvent e;
    //public delegate

    public void Trigger()
    {
        e?.Invoke();
    }
}
