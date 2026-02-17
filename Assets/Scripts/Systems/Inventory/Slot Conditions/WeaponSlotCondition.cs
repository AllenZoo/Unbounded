using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotCondition : IItemCondition
{
    public bool ConditionMet(Item item)
    {
        if (item == null || item.IsEmpty())
        {
            // 'No' item meets all conditions.
            // (We can have an empty weapon slot)
            return true;
        }

        // Check if Item has an attacker (weapon items have non-null attacker)
        return item.Data.attacker != null;
    }
}
