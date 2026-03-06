using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Assertions;


/// <summary>
/// Main Glue component of the MVC model. Connects view, controller, and initial model together.
/// 
/// View is required on instantiation.
/// Model can be empty.
/// </summary>
public class ObjectiveViewSystem : MonoBehaviour
{
    public event Action OnObjectiveCompleted;

    [Required, SerializeField] private ObjectiveView objectiveView;

    [SerializeField] private bool loadInitialObjectiveGroup = true;

    [ShowIf(nameof(loadInitialObjectiveGroup))]
    [Required, SerializeField] private ObjectiveGroupData data;

    private ObjectiveController objectiveController;

    private void Awake()
    {
        Assert.IsNotNull(objectiveView);

        if (data == null)
        {
            objectiveController = new ObjectiveController.Builder().WithoutInitialObjectiveGroup().Build(objectiveView);
            objectiveController.HideView();
        }
        else
        {
            objectiveController = new ObjectiveController.Builder().WithObjectiveGroup(data).Build(objectiveView);
            objectiveController.ShowView();
        }
    }

    private void Start()
    {
        OnObjectiveCompleted += HandleObjectiveCompleted;
    }

    private void OnEnable()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnObjectiveLoaded += HandleObjectiveLoaded;
        }

        if (objectiveController != null)
        {
            objectiveController.OnObjectiveCompleted += OnObjectiveCompleted;
        }
    }

    private void OnDisable()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnObjectiveLoaded -= HandleObjectiveLoaded;
        }

        if (objectiveController != null)
        {
            objectiveController.OnObjectiveCompleted -= OnObjectiveCompleted;
        }
        OnObjectiveCompleted -= HandleObjectiveCompleted;
    }

    public void Update()
    {
        objectiveController.Update(Time.deltaTime);
    }

    private void HandleObjectiveLoaded(LoadObjectiveRequest request)
    {
        if (request.objectives == null)
        {
            Debug.LogError("Received LoadObjectiveRequest with null objective group! This is not supported.");
            return;
        }

        objectiveController.UpdateModel(request.objectives);
    }

    private void HandleObjectiveCompleted()
    {
        ObjectiveManager.Instance.OnCurObjectiveComplete();
    }
}