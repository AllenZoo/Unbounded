using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For use with SerializedDictionary since it cannot serialize list of interfaces as values.
[CreateAssetMenu(fileName = "New Conditions", menuName = "Inventory/Slot Conditions")]
public class SO_Conditions : ScriptableObject
{
   public List<ConditionType> conditionTypes = new List<ConditionType>();

    private void OnValidate()
    {
        // Check for any duplicate enums
        for (int i = 0; i < conditionTypes.Count; i++)
        {
            for (int j = i + 1; j < conditionTypes.Count; j++)
            {
                if (conditionTypes[i] == conditionTypes[j])
                {
                    Debug.LogError("Duplicate condition type: " + conditionTypes[i] + " in SO_Condition object.");
                }
            }
        }
    }
}
