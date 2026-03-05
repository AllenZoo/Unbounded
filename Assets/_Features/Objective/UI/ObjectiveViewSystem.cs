using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Assertions;


/// <summary>
/// Main Glue component of the MVC model. Connects view, controller, and initial model together.
/// </summary>
public class ObjectiveViewSystem : MonoBehaviour
{
    [Required, SerializeField] private ObjectiveView objectiveView;
    [Required, SerializeField] private ObjectiveGroupData data;

    private ObjectiveController objectiveController;

    private void Awake()
    {
        Assert.IsNotNull(objectiveView);
        Assert.IsNotNull(data);

        objectiveController = new ObjectiveController.Builder().WithObjectiveGroup(data).Build(objectiveView);
        objectiveController.ShowView();
    }

    private void Start()
    {
        ObjectiveManager.Instance.OnObjectiveLoaded += HandleObjectiveLoaded;
    }

    private void HandleObjectiveLoaded(LoadObjectiveRequest request)
    {
        objectiveController.UpdateModel(request.objectives);
    }
}