using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Primary usage of this enum is to make serialization of conditions easier.
// When updated, make sure to also update ConditionTypeTranslator.
[SerializeField]
public enum ConditionType
{
    WEAPON_SLOT_CONDITION,
    UPGRADER_SLOT_CONDITION,
    EQUIPMENT_SLOT_CONDITION,
    NULL_SLOT_CONDITION
}
