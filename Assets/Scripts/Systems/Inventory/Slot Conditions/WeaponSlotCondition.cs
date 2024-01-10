using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotCondition : IItemCondition
{
    public bool ConditionMet(SO_Item item)
    {
        return item.weaponItem != null;
    }
}
