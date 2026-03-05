using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class ObjectiveViewSystem : MonoBehaviour
{
    [SerializeField, Required] private ObjectiveManagerContext context;
    [SerializeField, Required] private ObjectiveView objectiveView;
    [SerializeField, Required] private VisualTreeAsset taskItemTemplate;

    private ObjectiveManager objectiveManager;
    private readonly Dictionary<Objective, VisualElement> activeViews = new();

    private void Awake()
    {
        Assert.IsNotNull(context);
        Assert.IsNotNull(objectiveView);
        Assert.IsNotNull(taskItemTemplate);
    }

    private void Start()
    {
        objectiveManager = context.GetContext().Value;

        if (objectiveManager == null)
        {
            Debug.LogError("Objective Manager is null! Maybe context was not hooked up properly.");
            return;
        }

        objectiveManager.OnObjectiveActivated += HandleObjectiveActivated;
        objectiveManager.OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnDestroy()
    {
        if (objectiveManager != null)
        {
            objectiveManager.OnObjectiveActivated -= HandleObjectiveActivated;
            objectiveManager.OnObjectiveCompleted -= HandleObjectiveCompleted;
        }
    }

    private void HandleObjectiveActivated(Objective obj)
    {
        var element = objectiveView.AddTask(obj, taskItemTemplate);
        if (element != null)
        {
            activeViews.Add(obj, element);
            // Optionally update header on every new objective
            var data = obj.GetData();
            if (data != null)
                objectiveView.SetHeader("Main Objectives", data.ObjectiveName);
        }
    }

    private void HandleObjectiveCompleted(Objective obj)
    {
        if (activeViews.TryGetValue(obj, out var element))
        {
            objectiveView.UpdateTaskElement(element, obj);
        }
    }

    public void ClearAllViews()
    {
        objectiveView.ClearTasks();
        activeViews.Clear();
    }
}