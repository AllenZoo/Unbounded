using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class based on SOAP architecture. Whenever type of InteractblePromptData is modified, we fire event.
/// </summary>
// TODO: create this properly
[CreateAssetMenu()]
public class SO_InteractablePromptData : ScriptableObject
{
    public InteractablePromptData Data { get; private set; }
    public Action OnDataChanged;

    public void SetData(InteractablePromptData data)
    {
        Data = data;
        OnDataChanged?.Invoke();
    }
}
