using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Idea of the class is as follows.
/// 
/// We have two parts, the Bar UI and the data that the Bar UI references to display.
/// 
/// Since the data we require is related to some context data structure, we initialize the SO through there.
/// 
/// Once data is initialized, then we can display it on the UI.
/// 
/// </summary>
[CreateAssetMenu(fileName ="new Bar Context", menuName ="System/General UI/BarContext")]
public class BarContext : ScriptableObject
{
    public Action OnBarContextChange;

    public LocalEventHandler LEH { get { return leh; } set { leh = value; OnBarContextChange?.Invoke(); } }
    [SerializeField, ReadOnly] private LocalEventHandler leh;

    public bool IsVisible { get { return isVisible; } set { isVisible = value; OnBarContextChange?.Invoke(); } }
    [SerializeField, ReadOnly] private bool isVisible;
}
