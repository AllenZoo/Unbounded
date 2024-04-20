using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    FloorPlanGenerator floorPlanGenerator = new FloorPlanGenerator();

    /// <summary>
    /// Generate a new floor plan.
    /// Traverse through the rooms starting at dead ends and then linking them together.
    /// </summary>
    public void GenerateMap()
    {
        FloorPlan floorPlan = floorPlanGenerator.Generate();
    }

    /// <summary>
    /// Traverse through the floor plan and instantiate each room seperately.
    /// 
    /// </summary>
    public void InstantiateMap()
    {

    }

    // TODO:
    public void InstantiateRoom(Room room)
    {

    }

    /// <summary>
    /// Create a corridor between the two rooms.
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    public void InstantiateCorridor(Room room1, Room room2)
    {

    }
}
