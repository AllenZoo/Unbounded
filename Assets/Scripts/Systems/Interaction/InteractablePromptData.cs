using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InteractablePromptData
{
    [SerializeField]
    public string message;
    [SerializeField]
    public KeyCode reqKey; // Used in message for like (Press 'K' to interact)
    [SerializeField]
    public bool shouldDisplayPrompt;

    public InteractablePromptData(string message, KeyCode reqKey, bool display)
    {
        this.message = message;
        this.reqKey = reqKey;
        this.shouldDisplayPrompt = display;
    }
}
