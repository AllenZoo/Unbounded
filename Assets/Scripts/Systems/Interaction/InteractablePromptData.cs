using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InteractablePromptData
{
    [SerializeField]
    string message;
    [SerializeField]
    KeyCode reqKey; // Used in message for like (Press 'K' to interact)
}
