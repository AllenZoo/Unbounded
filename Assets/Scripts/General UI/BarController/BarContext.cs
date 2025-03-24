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
    //TODO: check if we go with this design or another one I thought of. REMOVE IF NOT USED.
    public Action OnInitialized;

    [SerializeField, ReadOnly] private LocalEventHandler localEventHandler;
    [SerializeField, ReadOnly] private StatComponent statObject;
    [SerializeField, ReadOnly] private bool initialized;

    public void Init()
    {
        initialized = true;
        OnInitialized();
    }
}
