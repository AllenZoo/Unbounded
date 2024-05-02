using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlan
{
    public Room[,] rooms;
    public HashSet<Room> deadEnds = new HashSet<Room>();

    public FloorPlan(int width, int height)
    {
        rooms = new Room[width, height];
    }
}
