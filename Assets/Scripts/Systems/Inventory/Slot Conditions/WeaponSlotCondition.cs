using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotCondition : IItemCondition
{
    public bool ConditionMet(Item item)
    {
        if (item == null || item.data == null)
        {
            // No item meets all conditions.
            return true;
        }

        return item.data.attacker != null;
    }
}
