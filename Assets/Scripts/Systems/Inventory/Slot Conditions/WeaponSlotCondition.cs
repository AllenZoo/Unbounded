using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotCondition : ICondition
{
    public bool ConditionMet(SO_Item item)
    {
        return item.weaponItem != null;
    }
}
