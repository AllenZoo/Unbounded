using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPlanGenerator : MonoBehaviour
{
    /// <summary>
    /// The floor plan is a 2D array of rooms. Each room has a position and a size.
    /// Origin is at the top left corner. (0, 0)
    /// </summary>
    protected Room[] floorplan;
    protected Vector2 floorplanSize = new Vector2(12, 12);
    protected int roomsToGenerate = 12;
    protected int roomsGenerated = 0;

    protected Queue<Room> roomsToVisit = new Queue<Room>();

    protected enum RoomSize
    {
        OneByOne,
        OneByTwo,
        TwoByOne,
        TwoByTwo
    }

    /// <summary>
    /// Generates a floor plan. Algorithm is as follows:
    /// 1. Create a start room somewhere in the floor plan and add it to the queue.
    /// 2. While we need to generate more rooms:
    ///     a. Pop a room from the queue.
    ///     b. Iterate through the room's potential exits.
    ///         i. 50% chance to give up creating a room for that exit.
    ///         ii. If we decide to create a room, check if a room can be created through that exit
    ///             (randomly selecting from 1x1, 1x2, 2x1, 2x2 sized rooms)
    ///             and create a room. Add it to the queue.
    ///         iii. If we have enough rooms generated, break.
    ///         iv. Add room to floorplan.
    ///         v. If room doesn't add any neighbouring rooms, mark it as a dead end.
    /// </summary>
    public Room[] Generate()
    {
        InitStartRoom();
        GenerateFloorPlan();
        return floorplan;
    }

    /// <summary>
    /// Clears current floor plan and then randomly creates a start room somewhere in the floor plan. A start room is a 1x1 room.
    /// </summary>
    private void InitStartRoom()
    {
        ClearFloorPlan();
        Vector2 randomPos = new Vector2(Random.Range(0, 12), Random.Range(0, 12));
        Room startRoom = new Room(new Vector2(1, 1), randomPos, null);
        AddRoomToFloorPlan(startRoom);
        roomsToVisit.Enqueue(startRoom);
    }

    /// <summary>
    /// Generates floor plan.
    /// </summary>
    private void GenerateFloorPlan()
    {
        // While we need to generate more rooms.
        while (roomsToGenerate < roomsGenerated) {
            // If no more rooms to visit, rerun.
            if (roomsToVisit.Count == 0)
            {
                Generate();
                break;
            }

            Room currentRoom = roomsToVisit.Dequeue();
            List<Vector2> neighbours = GetNeighbouringCells(currentRoom);
            foreach (Vector2 neighbour in neighbours)
            {
                Room newRoom = GenerateRoom(neighbour, currentRoom);
                if (newRoom != null)
                {
                    roomsToVisit.Enqueue(newRoom);
                    AddRoomToFloorPlan(newRoom);
                    roomsGenerated++;
                }
            }    
        }
    }

    /// <summary>
    /// Attempts to generate a room at given position. If successful, returns the room. Otherwise, returns null.
    ///     i. 50% chance to give up creating a room for that exit.
    ///     ii. If we decide to create a room, check if a room can be created through that exit
    ///             (randomly selecting from 1x1, 1x2, 2x1, 2x2 sized rooms)
    ///             and create a room. Add it to the queue.
    ///     
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Room GenerateRoom(Vector2 pos, Room parentRoom)
    {
        // Check if position to potentially generate room is valid.
        if (!isValidCellPos(pos) || !isEmptyCellPos(pos))
        {
            return null;
        }

        // i. 50% chance to give up creating a room for that exit.
        if (Random.Range(0, 2) == 0)
        {
            return null;
        }

        // ii. If we decide to create a room, check if a room can be created through that exit
        //     (randomly selecting from 1x1, 1x2, 2x1, 2x2 sized rooms)
        List<RoomSize> roomSizes = GetPossibleRoomSizes(pos);

        // Randomly select a room size from possible room sizes.
        RoomSize roomSizeToMake = roomSizes[Random.Range(0, roomSizes.Count)];
        Room newRoom;
        switch (roomSizeToMake)
        {
            default:
            case RoomSize.OneByOne:
                newRoom = new Room(new Vector2(1, 1), pos, parentRoom);
                break;
            case RoomSize.OneByTwo:
                newRoom = new Room(new Vector2(1, 2), pos, parentRoom);
                break;
            case RoomSize.TwoByOne:
                newRoom = new Room(new Vector2(2, 1), pos, parentRoom);
                break;
            case RoomSize.TwoByTwo:
                newRoom = new Room(new Vector2(2, 2), pos, parentRoom);
                break;
        }

        return newRoom;
    }

    /// <summary>
    /// Get possible room size to generate at given position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private List<RoomSize> GetPossibleRoomSizes(Vector2 position)
    {
        List<RoomSize> roomSizes = new List<RoomSize>();

        // 1x1
        if (isValidCellPos(position) && isEmptyCellPos(position))
        {
            roomSizes.Add(RoomSize.OneByOne);
        }

        // 1x2
        Vector2 posDOffset = position + new Vector2(0, 1);
        if (isValidCellPos(position) && isEmptyCellPos(position) 
            && isValidCellPos(posDOffset) && isEmptyCellPos(posDOffset))
        {
            roomSizes.Add(RoomSize.OneByTwo);
        }

        // 2x1
        Vector2 posROffset = position + new Vector2(1, 0);
        if (isValidCellPos(position) && isEmptyCellPos(position) 
            && isValidCellPos(posROffset) && isEmptyCellPos(posROffset))
        {
            roomSizes.Add(RoomSize.TwoByOne);
        }

        // 2x2
        if (isValidCellPos(position) && isEmptyCellPos(position) 
            && isValidCellPos(posDOffset) && isEmptyCellPos(posDOffset)
            && isValidCellPos(posROffset) && isEmptyCellPos(posROffset))
        {
            roomSizes.Add(RoomSize.TwoByTwo);
        }

        return roomSizes;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private List<Vector2> GetNeighbouringCells(Room room)
    {
        List<Vector2> neighbours = new List<Vector2>();

        for (int i = 0; i < room.size.x; i++)
        {
            for (int j = 0; j < room.size.y; j++)
            {
                // Get all valid neighbouring cells.
                // Note room.position is the top left cell.
                Vector2 cell = new Vector2(room.position.x + i, room.position.y + j);
                if (isValidCellPos(cell))
                {
                    neighbours.Add(cell);
                }
            }
        }
        return neighbours;
    }

    /// <summary>
    /// Helper to check if a cell is within the bounds of the floor plan. 
    /// Note floor plan is index-zero based.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns>whether cell is within floor plan or not.</returns>
    private bool isValidCellPos(Vector2 cell)
    {
        return cell.x >= 0 && cell.x < floorplanSize.x && cell.y >= 0 && cell.y < floorplanSize.y;
    }

    private bool isValidCellPos(Vector2 cell, Vector2 offset)
    {
        return isValidCellPos(cell + offset);
    }

    /// <summary>
    /// Helper to check if cell is empty at floor plan position cell.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool isEmptyCellPos(Vector2 cell)
    {
        return floorplan[(int) (cell.x + cell.y * floorplanSize.x)] == null;
    }

    /// <summary>
    /// Adds a room to the floor plan.
    /// </summary>
    /// <param name="room"></param>
    private void AddRoomToFloorPlan(Room room)
    {
        for (int i = 0; i < room.size.x; i++)
        {
            for (int j = 0; j < room.size.y; j++)
            {
                if (floorplan[(int) (room.position.x + i + (room.position.y + j) * floorplanSize.x)] != null)
                {
                    Debug.LogError("Room already exists at position: " + room.position);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Resets floor plan back to empty state. As a sideeffect also initializes the array.
    /// </summary>
    private void ClearFloorPlan()
    {
        floorplan = new Room[(int)floorplanSize.x * (int)floorplanSize.y];
    }
}
