using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemDescReqEvent : IGlobalEvent
{
    public Item item;
    public bool display; // could be false if player does not hover over item anymore.
}
