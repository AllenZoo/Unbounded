using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if a set of Boolean conditions match expected values.
/// </summary>
public class ConditionChecker : SerializedMonoBehaviour
{
    [DictionaryDrawerSettings(KeyLabel = "Condition", ValueLabel = "Expected Value:")]
    [SerializeField, Tooltip("Maps Boolean ScriptableObjects to their expected values.")]
    private Dictionary<SerializableObjectBoolean, bool> conditionMap = new();


    /// <summary>
    /// Validates that all conditions match their expected values.
    /// </summary>
    /// <returns>True if all conditions match, otherwise false.</returns>
    public bool ValidateConditions()
    {
        foreach (var (condition, expectedValue) in conditionMap)
        {
            if (condition.Value != expectedValue)
                return false;
        }

        return true;
    }
}

