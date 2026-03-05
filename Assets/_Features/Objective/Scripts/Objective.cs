using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objective Entry
/// </summary>
[Serializable]
public class Objective
{
    public ObjectiveState State => state;
    public ObjectiveData Data => data;


    [SerializeField] private ObjectiveState state;
    [SerializeField] private ObjectiveData data;

    public event Action<Objective> OnObjectiveComplete;

    public Objective(ObjectiveState state, ObjectiveData data)
    {
        this.state = state;
        this.data = data;
    }

    public bool IsEmpty() => data == null;
    public bool IsComplete() => state == ObjectiveState.COMPLETE;

    public void CompleteObjective()
    {
        if (state == ObjectiveState.ACTIVE)
        {
            state = ObjectiveState.COMPLETE;
            OnObjectiveComplete?.Invoke(this);
        }
    }

    #region Getters and Setters
    public ObjectiveState GetState()
    {
        return state;
    }
    public void SetState(ObjectiveState objectiveState)
    {
        this.state = objectiveState;
    }
    public ObjectiveData GetData() { return  data; }
    public void SetData(ObjectiveData data) {  this.data = data; }
    #endregion
}

public enum ObjectiveState
{
    INACTIVE,
    ACTIVE,
    COMPLETE,
    ABANDONED
}
