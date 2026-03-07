using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Responsible for handling the state of objectives, as well as the main entrypoint for activating and completing objectives.
/// </summary>
public class ObjectiveManager : Singleton<ObjectiveManager>, IDataPersistence
{
    public event Action<LoadObjectiveRequest> OnObjectiveLoaded;


    private EventBinding<LoadObjectiveRequest> lorBinding;
    private ObjectiveGroup curObjectiveGroup;

    protected override void Awake()
    {
        base.Awake();
        lorBinding = new EventBinding<LoadObjectiveRequest>(LoadObjectiveRequestHandler);
        curObjectiveGroup = null;
    }
    private void OnEnable()
    {
        if (lorBinding != null)
        {
            EventBus<LoadObjectiveRequest>.Register(lorBinding);
        }
    }
    private void OnDisable()
    {
        if (lorBinding != null)
        {
            EventBus<LoadObjectiveRequest>.Unregister(lorBinding);
        }
    }

    private void LoadObjectiveRequestHandler(LoadObjectiveRequest request)
    {
        var objectiveGroup = request.objectives;

        if (curObjectiveGroup != null)
        {
            Debug.LogError("Received LoadObjectiveRequest while already having an active objective group! This is not supported.");
            return;
        }

        curObjectiveGroup = new ObjectiveGroup(objectiveGroup);
        OnObjectiveLoaded?.Invoke(request);
    }

    public void OnCurObjectiveComplete()
    {
        Debug.Log("Here!");
        curObjectiveGroup = null;
    }

    public void LoadData(GameData data)
    {
        
    }

    public void SaveData(GameData data)
    {
        
    }
}
