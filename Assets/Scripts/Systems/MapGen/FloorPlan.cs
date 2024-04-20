using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlan
{
    public Room[,] floorplan;
    public HashSet<Room> deadEnds = new HashSet<Room>();

    public FloorPlan(int width, int height)
    {
        floorplan = new Room[width, height];
    }
}
