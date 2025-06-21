using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Utility class for mapping an enum condition type to a concrete condition.
public class ConditionTypeTranslator : Singleton<ConditionTypeTranslator>
{
    private Dictionary<ConditionType, IItemCondition> conditionTypeToCondition;

    private void Awake()
    {
        base.Awake();

        Init();
    }

    // Update this function when adding new condition types.
    private void Init()
    {
        conditionTypeToCondition = new Dictionary<ConditionType, IItemCondition>();

        // Add more mapping of condition type to condition here.
        conditionTypeToCondition.Add(ConditionType.WEAPON_SLOT_CONDITION, new WeaponSlotCondition());
        conditionTypeToCondition.Add(ConditionType.UPGRADER_SLOT_CONDITION, new UpgraderSlotCondition());
        conditionTypeToCondition.Add(ConditionType.NULL_SLOT_CONDITION, new NullSlotCondition());
        conditionTypeToCondition.Add(ConditionType.EQUIPMENT_SLOT_CONDITION, new EquipmentSlotCondition());
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
