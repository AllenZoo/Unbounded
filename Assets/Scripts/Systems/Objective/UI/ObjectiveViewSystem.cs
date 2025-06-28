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
    private readonly List<ObjectiveView> activeViews = new();

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
        activeViews.Add(view);
    }

    private void HandleObjectiveCompleted(Objective obj)
    {
        // Optionally, fade out or remove the view.
        var view = activeViews.Find(v => v.name == obj.GetData().ObjectiveName); // Better: compare by ID
        if (view != null)
        {
            Destroy(view.gameObject);
            activeViews.Remove(view);
        }
    }

    public void ClearAllViews()
    {
        foreach (var view in activeViews)
        {
            Destroy(view.gameObject);
        }
        activeViews.Clear();
    }
}
