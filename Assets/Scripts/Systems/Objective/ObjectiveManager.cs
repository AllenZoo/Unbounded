using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public event Action<Objective> OnObjectiveActivated;
    public event Action<Objective> OnObjectiveCompleted;


    [Tooltip("List of tutorial objectives, in order of execution.")]
    [SerializeField] private List<Objective> tutorialObjectives;
    private Objective curActive;


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

        if (!curState.Equals(ObjectiveState.INACTIVE))
        {
            Debug.LogError("Tried to activate an objective that is not inactive!");
            return null;
        }

        obj.SetState(ObjectiveState.ACTIVE);
        var context = obj.GetData().HighlightableContext.GetContext().Value;

        if (context != null)
        {
            context.Highlight();
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
            ActivateObjective(curActive);
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
            if (obj.GetState().Equals(ObjectiveState.INACTIVE))
            {
                return obj;
            }
        }
        return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Here in update!");
            //ActivateObjective(test);
        }
    }
}
