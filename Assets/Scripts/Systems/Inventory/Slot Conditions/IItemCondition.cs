using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IItemCondition
{
    public bool ConditionMet(SO_Item item);
}
