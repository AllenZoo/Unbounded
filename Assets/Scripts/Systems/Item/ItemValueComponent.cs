using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: eventually split item value component into one for forging costs and another for selling values.
[Serializable]
public class ItemValueComponent : IItemComponent
{
    public int goldValue = 1;
}
