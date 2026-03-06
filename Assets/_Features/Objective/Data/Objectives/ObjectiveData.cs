using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static Objective Data.
/// </summary>
[CreateAssetMenu(fileName ="new objective data", menuName ="System/Objective/ObjectiveData")]
public class ObjectiveData : ScriptableObject
{
    public string ObjectiveName;

    [TextArea(5, 8)]
    public string ObjectiveText;

    [Tooltip("Ref to highlightable that will be highlighted when objective is active. Can be null.")]
    public HighlightableContext HighlightableContext;

    [Tooltip("List of conditions for objective completion. All conditions must be satisfied for the objective to be completed.")]
    public List<ObjectiveConditionData> Conditions;

    [TextArea(5, 8), Tooltip("Description for design purposes")]
    public string ObjectiveDescription;
}
