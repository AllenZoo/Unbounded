using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles all objective functionality. Basically a tutorial manager.
/// </summary>
public class ObjectiveManager : MonoBehaviour, IDataPersistence
{
    public UnityEvent OnTutorialComplete;
    public event Action<Objective> OnObjectiveActivated;
    public event Action<Objective> OnObjectiveCompleted;


    [Tooltip("The SO that keeps track of whether the tutorial is complete or not.")]
    [SerializeField] private ScriptableObjectBoolean TutorialStateBoolean;

    [Tooltip("List of tutorial objectives, in order of execution.")]
    [SerializeField] private List<Objective> tutorialObjectives;
    private Objective curActive;

    private void Awake()
    {
        EventBinding<OnTutorialObjectiveRequest> binding = new EventBinding<OnTutorialObjectiveRequest>(StartUpTutorialObjectives);
        EventBus<OnTutorialObjectiveRequest>.Register(binding);
    }


    /// <summary>
    /// Main entrypoint function to call when we want to start/continue tutorial objectives.
    /// </summary>
    public void StartUpTutorialObjectives()
    {
        var obj = GetNextTutorialObjective();
        if (obj == null)
        {
            Debug.Log("No more tutorial objectives to activate.");
            return;
        }
        curActive = ActivateObjective(obj);
    }

    /// <summary>
    /// Activates an objective. Only activates objectives with "INACTIVE" state. Triggers the highlight for any relevant components.
    /// </summary>
    /// <param name="obj"></param>
    public Objective ActivateObjective(Objective obj)
    {
        var curState = obj.GetState();

        // Guards against bad objectives.
        if (!curState.Equals(ObjectiveState.INACTIVE))
        {
            Debug.LogError("Tried to activate an objective that is not inactive!");
            return null;
        }

        if (obj.IsEmpty())
        {
            Debug.LogError("Tried to activate an objective that is has no data!");
            return null;
        }

        obj.SetState(ObjectiveState.ACTIVE);

        var objPoint = obj.GetData().ObjectivePointContext.GetContext().Value;
        if (objPoint != null)
        {
            objPoint.SetObjective(obj);
            // TODO: currently we just advance to next objective once objective point is complete. May need to revamp if we ever make objectives with multiple
            //       objective points but leave for now.
            objPoint.OnObjectivePointComplete += (_) => CompleteAndAdvanceCurrent(); 
        }


        var highlight = obj.GetData().HighlightableContext.GetContext().Value;

        if (highlight != null)
        {
            highlight.Highlight();
        }


        OnObjectiveActivated?.Invoke(obj);
        return obj;
    }

    /// <summary>
    /// Completes an objective. Only completes objectives with "ACTIVE" state. Stops any highlighting of objectives.
    /// </summary>
    /// <param name="obj"></param>
    public Objective CompleteObjective(Objective obj)
    {
        var curState = obj.GetState();

        if (!curState.Equals(ObjectiveState.ACTIVE))
        {
            Debug.LogError("Tried to complete an objective that is not active!");
            return null;
        }


        obj.SetState(ObjectiveState.COMPLETE);
        var context = obj.GetData().HighlightableContext.GetContext().Value;

        if (context != null)
        {
            context.StopHighlight();
        }

        OnObjectiveCompleted?.Invoke(obj);
        return obj;
    }

    public void CompleteAndAdvanceCurrent()
    {
        if (curActive == null)
        {
            Debug.LogWarning("No current objective to complete.");
            return;
        }

        CompleteObjective(curActive);
        curActive = GetNextTutorialObjective();

        if (curActive != null)
        {
            ActivateObjective(curActive);
        } else
        {
            Debug.Log("Completed all tutorial objectives!");
            OnTutorialComplete?.Invoke();
            TutorialStateBoolean.Set(true);
        }
            
    }

    public void ResetTutorial()
    {
        foreach (var obj in tutorialObjectives)
        {
            obj.SetState(ObjectiveState.INACTIVE);
        }

        curActive = null;
    }


    /// <summary>
    /// Returns the next INACTIVE objective or null if none.
    /// </summary>
    /// <returns></returns>
    private Objective GetNextTutorialObjective()
    {
        foreach (var obj in tutorialObjectives)
        {
            if (obj.GetState().Equals(ObjectiveState.INACTIVE) && !obj.IsEmpty())
            {
                return obj;
            }
        }
        Debug.Log("No more tutorial objectives to get");
        return null;
    }


    public void LoadData(GameData data)
    {
        this.TutorialStateBoolean.Set(data.tutorialComplete);

        foreach (var obj in tutorialObjectives)
        {
            if (obj.IsEmpty()) continue;

            if (data.objectiveStates.TryGetValue(obj.GetData().name, out ObjectiveState savedState))
            {
                obj.SetState(savedState);
            }
        }

        // If an objective was active, we need to re-initialize its logic/UI
        curActive = tutorialObjectives.Find(o => o.GetState() == ObjectiveState.ACTIVE);
        if (curActive != null)
        {
            // We temporarily set it to INACTIVE to satisfy the ActivateObjective check,
            // or better yet, refactor the core activation logic into a helper.
            curActive.SetState(ObjectiveState.INACTIVE);
            ActivateObjective(curActive);
        }
    }

    public void SaveData(GameData data)
    {
        data.tutorialComplete = TutorialStateBoolean.Value;
        data.objectiveStates.Clear();

        foreach (var obj in tutorialObjectives)
        {
            if (!obj.IsEmpty())
            {
                data.objectiveStates[obj.GetData().name] = obj.GetState();
            }
        }
    }
}
