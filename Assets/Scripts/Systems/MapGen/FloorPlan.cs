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

    /// <summary>
    /// Adds a room to the floor plan.
    /// </summary>
    /// <param name="room"></param>
    public void AddRoom(Room room)
    {
        for (int i = 0; i < room.size.x; i++)
        {
            for (int j = 0; j < room.size.y; j++)
            {
                if (rooms[(int)(room.position.x + i), (int)(room.position.y + j)] != null)
                {
                    Debug.LogError("Room already exists at position: " + room.position);
                    return;
                }
                else
                {
                    rooms[(int)(room.position.x + i), (int)(room.position.y + j)] = room;
                }
            }
        }
    }

    public void RemoveRoom(Room room)
    {
        // TODO:
    }
}
