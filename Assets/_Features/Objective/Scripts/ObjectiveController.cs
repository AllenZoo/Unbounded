using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main controller class that handles the logic for updating the objective view based on changes to the objective model.
/// </summary>
public class ObjectiveController
{
    public event Action OnObjectiveCompleted;

    private ObjectiveView view;
    private ObjectiveGroup model;

    public ObjectiveController(ObjectiveView view, ObjectiveGroup group)
    {
        this.view = view;
        this.model = group;

        ConnectModel();
        ConnectView();
    }

    public class Builder
    {
        readonly ObjectiveGroup model = new ObjectiveGroup();

        public Builder WithObjectiveGroup(ObjectiveGroupData data)
        {
            model.Initialize(data);
            return this;
        }

        public Builder WithoutInitialObjectiveGroup()
        {
            return this;
        }

        public ObjectiveController Build(ObjectiveView view)
        {
            if (view == null)
            {
                Debug.LogError("ObjectiveView is null! Cannot build ObjectiveController.");
                return null;

            }
            return new ObjectiveController(view, model);
        }
    }

    /// <summary>
    /// Updates the model with the specified objective group data and refreshes the view to reflect the changes.
    /// </summary>
    /// <param name="newGroup">The new objective group data to initialize the model with. Cannot be null.</param>
    public void UpdateModel(ObjectiveGroupData newGroup)
    {
        model.Initialize(newGroup);
        ShowView();
    }

    public void HideView()
    {
        view.gameObject.SetActive(false);
    }
    public void ShowView()
    {
        view.gameObject.SetActive(true);

        if (model != null && !model.IsEmpty())
        {
            var config = GenerateViewConfig(model);
            view.UpdateView(config);
        }
    }

    public void Update(float deltaTime)
    {
        model.Update(deltaTime);
    } 

    private void ConnectModel()
    {
        // For any event subscriptions necessary.
        model.OnStateChanged += HandleModelUpdate;
    }
    private void ConnectView()
    {
        // For any event subscriptions necessary.
    }

    private void HandleModelUpdate()
    {
        if (model != null && !model.IsEmpty())
        {
            if (model.IsComplete()) {
                OnObjectiveCompleted?.Invoke(); 
            }

            // View also has access to whether Objective is complete, so we can just pass the config and let it decide how to display based on that state.
            // e.g. when objective is complete, it will dissolve the container.
            var config = GenerateViewConfig(model);
            view.UpdateView(config);
        }
    }

    /// <summary>
    /// Helper function to package given event payload into a config object that can be easily consumed by the view. 
    /// This is where any necessary formatting or data transformation should occur before passing it to the view.
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    private ObjectiveViewConfig GenerateViewConfig(ObjectiveGroup group)
    {
        var headerTitle = "<u>Main Objectives</u>";
        var headerSubtitle = group.Data.GroupName;
        var taskItems = new List<TaskItemConfig>();

        var currentObjective = group.Objectives;
        if (currentObjective != null)
        {
            foreach (var task in currentObjective)
            {
                taskItems.Add(new TaskItemConfig(task.Data.ObjectiveText, task.IsComplete()));
            }
        }
        return new ObjectiveViewConfig(headerTitle, headerSubtitle, taskItems);
    }

    private ObjectiveViewConfig GenerateEmptyViewConfig()
    {
        var headerTitle = "<u>Main Objectives</u>";
        var headerSubtitle = "All Done";
        var taskItems = new List<TaskItemConfig>();
        return new ObjectiveViewConfig(headerTitle, headerSubtitle, taskItems);
    }
}
