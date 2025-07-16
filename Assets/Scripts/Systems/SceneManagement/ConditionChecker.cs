using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component attached to objects that require a condition checker reference.
/// 
/// Class handles and checks if a set of Boolean conditions match expected values.
/// </summary>
public class ConditionChecker : SerializedMonoBehaviour
{
    [SerializeField]
    private List<BooleanCondition> conditions = new();


    /// <summary>
    /// Validates that all conditions match their expected values.
    /// </summary>
    /// <returns>True if all conditions match, otherwise false.</returns>
    public bool ValidateConditions()
    {
        foreach (var entry in conditions)
        {
            if (entry.Condition == null) continue;

            if (entry.Condition.Value != entry.ExpectedValue)
                return false;
        }

        return true;
    }

}

[System.Serializable]
public class BooleanCondition
{
    [HorizontalGroup("Split"), LabelWidth(100), InlineEditor, HideLabel]
    public SerializableObjectBoolean Condition;

    [HorizontalGroup("Split"), LabelWidth(100)]
    public bool ExpectedValue;
}


