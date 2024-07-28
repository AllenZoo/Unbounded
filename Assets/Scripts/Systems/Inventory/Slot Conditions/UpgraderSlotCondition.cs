using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgraderSlotCondition : IItemCondition
{
    public bool ConditionMet(Item item)
    {
        if (item == null || item.IsEmpty())
        {
            // 'No' item meets all conditions.
            // (We can have an empty upgrade slot)
            return true;
        }
        
        // Check if Item data is type of SO_Upgrade_Item.
        return item.HasComponent<ItemUpgraderComponent>();
    }
}
