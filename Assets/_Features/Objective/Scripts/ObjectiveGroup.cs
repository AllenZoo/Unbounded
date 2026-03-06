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
        data = new ObjectiveGroupData();
        objectives = new List<Objective>();
    }

    public ObjectiveGroup(ObjectiveGroupData data)
    {
       Initialize(data);
    }

    public void Initialize(ObjectiveGroupData data)
    {
        this.data = data;
        objectives = new List<Objective>();
        foreach (var objectiveData in data.Objectives)
        {
            objectives.Add(new Objective(ObjectiveState.ACTIVE, objectiveData));
        }
    }

    public bool IsEmpty() => data == null || objectives.Count == 0;
}
