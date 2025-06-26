using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private ObjectiveData test;

    public void ActivateObjective(ObjectiveData data)
    {
        //Debug.Log("Activating Objective!");
        var context = data.HighlightableContext.GetContext().Value;

        if (context != null)
        {
            //Debug.Log("Activating Highlight!");
            context.Highlight();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("Here in update!");
            ActivateObjective(test);
        }
    }
}
