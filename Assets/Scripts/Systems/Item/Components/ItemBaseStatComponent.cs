using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBaseStatComponent : IItemComponent
{
    List<Tuple<Stat, int>> baseStatList = new List<Tuple<Stat, int>>();
}
