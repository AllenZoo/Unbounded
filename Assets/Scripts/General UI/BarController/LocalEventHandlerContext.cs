using Sirenix.OdinInspector;
using System;
using UnityEngine;

/// </summary>
[CreateAssetMenu(fileName = "new LEH Context", menuName = "System/General UI/LEH Context")]
public class LocalEventHandlerContext : ScriptableObject
{
    //TODO: check if we go with this design or another one I thought of. REMOVE IF NOT USED.
    public Action OnInitialized;

    public LocalEventHandler LocalEventHandler { get { return localEventHandler; } private set { } }
    public bool Initialized { get { return initialized; } private set { } }

    [SerializeField, ReadOnly] private LocalEventHandler localEventHandler;
    [SerializeField, ReadOnly] private bool initialized;

    public void Init(LocalEventHandler localEventHandler)
    {
        this.localEventHandler = localEventHandler;
        initialized = true;
        OnInitialized?.Invoke();
    }
}
