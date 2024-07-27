using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Utility class for mapping an enum condition type to a concrete condition.
public class ConditionTypeTranslator : MonoBehaviour
{
    public static ConditionTypeTranslator Instance;

    private Dictionary<ConditionType, IItemCondition> conditionTypeToCondition;

    private void Awake()
    {
        // Check only one instance of this class exists
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.transform.parent.gameObject);
        }

        Init();
    }

    // Update this function when adding new condition types.
    private void Init()
    {
        conditionTypeToCondition = new Dictionary<ConditionType, IItemCondition>();

        // Add more mapping of condition type to condition here.
        conditionTypeToCondition.Add(ConditionType.WEAPON_SLOT_CONDITION, new WeaponSlotCondition());
        conditionTypeToCondition.Add(ConditionType.UPGRADER_SLOT_CONDITION, new UpgraderSlotCondition());
    }

    public IItemCondition Translate(ConditionType conditionType)
    {
        if (conditionTypeToCondition.ContainsKey(conditionType))
        {
            return conditionTypeToCondition[conditionType];
        }
        else
        {
            Debug.LogError("ConditionTypeTranslator does not contain mapping for condition type: " + conditionType);
            return null;
        }
    }
}
