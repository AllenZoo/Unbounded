using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlan
{
    public Room[,] rooms;
    public HashSet<Room> DeadEnds { get { return GetAllDeadEnds(); } private set { } }

    // Contains a list of refs to all rooms included in floor plan so far.
    private HashSet<Room> roomList = new HashSet<Room>();

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
        roomList.Add(room);
    }

    public void RemoveRoom(Room room)
    {
        if (room == null)
            return;

        for (int i = 0; i < room.size.x; i++)
        {
            for (int j = 0; j < room.size.y; j++)
            {
                if (rooms[(int)(room.position.x + i), (int)(room.position.y + j)] == room)
                {
                    rooms[(int)(room.position.x + i), (int)(room.position.y + j)] = null;
                }
            }
        }

        roomList.Remove(room);
    }

    public void SwapRoom(Room oldRoom, Room newRoom)
    {
        if (oldRoom == null || newRoom == null) return;

        RemoveRoom(oldRoom);  // Remove the old room from the grid

        newRoom.parent = oldRoom.parent; // Make sure the new room has same parent as old room.

        AddRoom(newRoom);     // Add the new room to the grid
    }

    public HashSet<Room> GetAllDeadEnds()
    {
        // For every room, find all the dead ends.
        // A room is a deadend, if it is not a parent of any other room!
        HashSet<Room> result = new HashSet<Room>(roomList);

        foreach (Room room in roomList)
        {
            result.Remove(room.parent);
        }
        return result;
    }
}
