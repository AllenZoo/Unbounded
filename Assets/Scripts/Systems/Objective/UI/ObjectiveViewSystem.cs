using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ObjectiveViewSystem : MonoBehaviour
{
    [SerializeField, Required] private ObjectiveManagerContext context;
    [SerializeField, Required] private ObjectiveView pfbObjectiveView;
    [SerializeField, Required] private Transform objectiveListParent;

    private ObjectiveManager objectiveManager;
    private readonly Dictionary<Objective, ObjectiveView> activeViews = new();

    private void Awake()
    {
        Assert.IsNotNull(context);
        Assert.IsNotNull(pfbObjectiveView);
        Assert.IsNotNull(objectiveListParent);
    }

    private void Start()
    {
        objectiveManager = context.GetContext().Value;

        if (objectiveManager == null)
        {
            Debug.LogError("Objective Manager is null! Maybe context was not hooked up properly.");
        }

        objectiveManager.OnObjectiveActivated += HandleObjectiveActivated;
        objectiveManager.OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnDestroy()
    {
        objectiveManager.OnObjectiveActivated -= HandleObjectiveActivated;
        objectiveManager.OnObjectiveCompleted -= HandleObjectiveCompleted;
    }

    private void HandleObjectiveActivated(Objective obj)
    {
        var view = Instantiate(pfbObjectiveView, objectiveListParent);
        view.SetData(obj);
        activeViews.Add(obj, view);
    }

    private void HandleObjectiveCompleted(Objective obj)
    {
        // Optionally, fade out or remove the view.
        var view = activeViews[obj];
        if (view != null)
        {
            Destroy(view.gameObject);
            activeViews.Remove(obj);
        }
    }

    public void ClearAllViews()
    {
        foreach (var kvp in activeViews)
        {
            Destroy(kvp.Value.gameObject);
        }
        activeViews.Clear();
    }
}
