using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveGroup
{
    public event Action OnStateChanged;

    public ObjectiveGroupData Data => data;
    [SerializeField] private ObjectiveGroupData data;

    public List<Objective> Objectives => objectives;
    private List<Objective> objectives;
    

    public ObjectiveGroup ()
    {
        data = ScriptableObject.CreateInstance<ObjectiveGroupData>();
        objectives = new List<Objective>();
    }

    public ObjectiveGroup(ObjectiveGroupData data)
    {
       Initialize(data);
    }

    public void Initialize(ObjectiveGroupData data)
    {
        this.data = data;

        // Redundent Cleanup. (But just in case)
        Cleanup();

        objectives = new List<Objective>();
        foreach (var objectiveData in data.Objectives)
        {
            var curObjective = new Objective(ObjectiveState.ACTIVE, objectiveData);
            curObjective.OnObjectiveComplete += HandleObjectiveComplete;
            objectives.Add(curObjective);
        }
    }

    public void Cleanup()
    {
        if (objectives == null) return;

        foreach (var objective in objectives)
        {
            objective.OnObjectiveComplete -= HandleObjectiveComplete;
        }
        objectives.Clear();
    }

    public void Update(float deltaTime)
    {
        foreach (var objective in objectives)
        {
            if (objective.State == ObjectiveState.ACTIVE)
            {
                objective.Update(deltaTime);
            }
        }
    }

    public bool IsEmpty() => data == null || objectives.Count == 0;
    public bool IsComplete() => objectives.TrueForAll(obj => obj.State == ObjectiveState.COMPLETE);

    private void HandleObjectiveComplete()
    {
        // Chain Event.
        OnStateChanged?.Invoke();
    }


}
