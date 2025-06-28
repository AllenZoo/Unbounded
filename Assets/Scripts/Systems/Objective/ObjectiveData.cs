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

    [Tooltip("Ref to objective point that will be invoke an event to complete objective.")]
    public ObjectivePointContext ObjectivePointContext;

    [TextArea(5, 8), Tooltip("Description for design purposes")]
    public string ObjectiveDescription;
}
