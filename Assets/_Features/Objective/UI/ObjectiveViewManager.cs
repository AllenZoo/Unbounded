using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// This class is now obsolete as part of the UI Toolkit refactor.
/// Use ObjectiveView directly on your master UI Document.
/// </summary>
public class ObjectiveViewManager : MonoBehaviour
{
    [SerializeField, Required] private ObjectiveView masterView;
    [SerializeField, ReadOnly] private Objective data;

    public void SetViewData(Objective data)
    {
        this.data = data;
        // masterView is now responsible for the whole UI list. 
        // Logic for single row is handled in ObjectiveView.AddTask()
    }
}