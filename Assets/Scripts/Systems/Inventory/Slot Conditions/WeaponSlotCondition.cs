using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotCondition : IItemCondition
{
    public bool ConditionMet(Item item)
    {
        return item.data.attacker != null;
    }
}
