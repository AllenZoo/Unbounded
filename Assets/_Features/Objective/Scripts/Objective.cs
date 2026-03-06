using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Objective
{
    public ObjectiveState State => state;
    public ObjectiveData Data => data;
    public List<IObjectiveCondition> Conditions { get; private set; } = new List<IObjectiveCondition>();

    [SerializeField] private ObjectiveState state;
    [SerializeField] private ObjectiveData data;

    public event Action OnObjectiveComplete;

    public Objective(ObjectiveState state, ObjectiveData data)
    {
        this.state = state;
        this.data = data;

        if (data != null && data.Conditions != null)
        {
            foreach (var conditionData in data.Conditions)
            {
                if (conditionData == null) continue;
                var condition = conditionData.CreateInstance();
                if (condition == null) continue;
                condition.Initialize(this);
                condition.OnStateChanged += CheckCompletion;
                Conditions.Add(condition);
            }
        }
    }

    public bool IsEmpty() => data == null;
    public bool IsComplete() => state == ObjectiveState.COMPLETE;

    public void CompleteObjective()
    {
        if (state == ObjectiveState.ACTIVE)
        {
            state = ObjectiveState.COMPLETE;
            OnObjectiveComplete?.Invoke();
            
            foreach(var condition in Conditions) {
                condition.Cleanup();
            }
        }
    }
    public void CheckCompletion()
    {
        if (state != ObjectiveState.ACTIVE) return;

        bool allMet = true;
        foreach (var condition in Conditions)
        {
            if (!condition.IsMet())
            {
                allMet = false;
                break;
            }
        }

        if (allMet)
        {
            CompleteObjective();
        }
    }

    public void Update(float deltaTime)
    {
        if (state != ObjectiveState.ACTIVE) return;
        foreach (var condition in Conditions)
        {
            condition.Update(deltaTime);
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
    public ObjectiveData GetData() { return data; }
    public void SetData(ObjectiveData data) { this.data = data; }
    #endregion
}
public enum ObjectiveState
{
    INACTIVE,
    ACTIVE,
    COMPLETE,
    ABANDONED
}