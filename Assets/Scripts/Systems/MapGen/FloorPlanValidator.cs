using System.Collections.Generic;
using System.Linq;
/* Unmerged change from project 'Assembly-CSharp.Player'
Before:
using System.Linq;
After:
using UnityEngine;
*/


public class FloorPlanValidator
{
    /// <summary>
    /// Floor plan to validate.
    /// </summary>
    protected FloorPlan floorPlan;

    /// <summary>
    /// Whether to log out error statements if validation fails
    /// </summary>
    private bool verbose = false;

    public FloorPlanValidator(FloorPlan floorPlan)
    {
        this.floorPlan = floorPlan;
    }


    /// <summary>
    /// A valid floorplan is defined as such:
    ///     i. having the correct # of rooms.
    ///     ii. having only 1 boss room.
    ///     iii. the rooms are connected properly (no diagonal linkage)
    ///     iv. check that roomList.Count matches actual amount of rooms in floorplan for weird bugs.
    /// </summary>
    /// <returns>True if the floor plan is valid, false otherwise</returns>
    public bool ValidateFloorPlan()
    {
        // First, make sure we have a floorplan to validate
        if (floorPlan == null)
        {
            if (verbose) Debug.LogError("FloorPlanValidator: Floor plan is null!");

            return false;
        }

        // i. Count the number of actual rooms in the grid
        int roomCount = CountRoomsInGrid();

        // iv. Check that roomList.Count matches actual amount of rooms in floorplan
        if (roomCount != floorPlan.roomList.Count)
        {
            if (verbose) Debug.LogError($"FloorPlanValidator: Room count mismatch! Grid has {roomCount} rooms, roomList has {floorPlan.roomList.Count} rooms.");
            return false;
        }

        // ii. Having only 1 boss room
        int bossRoomCount = CountBossRooms();
        if (bossRoomCount != 1)
        {
            if (verbose) Debug.LogError($"FloorPlanValidator: Found {bossRoomCount} boss rooms, expected exactly 1.");
            return false;
        }

        // iii. The rooms are connected properly (no diagonal linkage)
        if (!IsBossRoomProperlyConnected())
        {
            if (verbose) Debug.LogError("FloorPlanValidator: Rooms are not properly connected!");
            return false;
        }

        // All validation checks passed
        if (verbose) Debug.Log("FloorPlanValidator: Floor plan is valid!");
        return true;
    }

    /// <summary>
    /// Counts the number of unique room references in the grid
    /// </summary>
    private int CountRoomsInGrid()
    {
        HashSet<Room> uniqueRooms = new HashSet<Room>();
        int rows = floorPlan.rooms.GetLength(0);
        int cols = floorPlan.rooms.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Room room = floorPlan.rooms[i, j];
                if (room != null)
                {
                    uniqueRooms.Add(room);
                }
            }
        }

        return uniqueRooms.Count;
    }

    /// <summary>
    /// Counts the number of boss rooms in the floor plan
    /// </summary>
    private int CountBossRooms()
    {
        int count = 0;
        int rows = floorPlan.rooms.GetLength(0);
        int cols = floorPlan.rooms.GetLength(1);

        HashSet<Room> checkedRooms = new HashSet<Room>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Room room = floorPlan.rooms[i, j];
                if (room != null && !checkedRooms.Contains(room))
                {
                    checkedRooms.Add(room);
                    if (room.roomType == RoomType.Boss)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    /// <summary>
    /// Checks if all boss room is properly connected (no diagonal connections).
    /// 
    /// Needed since sometimes we transform a room into a 2x2 that misaligns it from the parent room.
    /// 
    /// A room is properly connected if it adjacent to its parent room.
    /// </summary>
    private bool IsBossRoomProperlyConnected()
    {
        // Check if boss room is properly connected to at least one other room
        Room bossRoom = FindBossRoom();
        if (bossRoom == null)
            return false;

        Room parentRoom = bossRoom.parent;

        Vector2 position = bossRoom.position;
        Room[,] rooms = floorPlan.rooms;
        bool properlyConnected = false;

        // Since boss room is assumed to be 2x2, we need to check the perimeter
        // around it for neighboring rooms

        int bossX = (int)position.x;
        int bossY = (int)position.y;

        // Check left side (two cells)
        if (bossX > 0)
        {
            if (rooms[bossX - 1, bossY] == parentRoom ||
                rooms[bossX - 1, bossY + 1] == parentRoom)
            {
                properlyConnected = true;
            }
        }

        // Check right side (two cells)
        if (bossX + 2 < rooms.GetLength(0))
        {
            if (rooms[bossX + 2, bossY] == parentRoom ||
                rooms[bossX + 2, bossY + 1] == parentRoom)
            {
                properlyConnected = true;
            }
        }

        // Check top side (two cells)
        if (bossY > 0)
        {
            if (rooms[bossX, bossY - 1] == parentRoom ||
                rooms[bossX + 1, bossY - 1] == parentRoom)
            {
                properlyConnected = true;
            }
        }

        // Check bottom side (two cells)
        if (bossY + 2 < rooms.GetLength(1))
        {
            if (rooms[bossX, bossY + 2] == parentRoom ||
                rooms[bossX + 1, bossY + 2] == parentRoom)
            {
                properlyConnected = true;
            }
        }

        return properlyConnected;
    }

    /// <summary>
    /// Finds the start room in the floor plan
    /// </summary>
    private Room FindStartRoom()
    {
        int rows = floorPlan.rooms.GetLength(0);
        int cols = floorPlan.rooms.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Room room = floorPlan.rooms[i, j];
                if (room != null && room.roomType == RoomType.Start)
                {
                    return room;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Finds the start room in the floor plan
    /// </summary>
    private Room FindBossRoom()
    {
        int rows = floorPlan.rooms.GetLength(0);
        int cols = floorPlan.rooms.GetLength(1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Room room = floorPlan.rooms[i, j];
                if (room != null && room.roomType == RoomType.Boss)
                {
                    return room;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all rooms that are adjacent to the given room
    /// </summary>
    private List<Room> GetAdjacentRooms(Room room)
    {
        List<Room> adjacentRooms = new HashSet<Room>().ToList();
        List<Vector2> cellPositions = room.GetCellPositions();

        foreach (Vector2 cellPos in cellPositions)
        {
            // Check all four directions
            Vector2[] directions = new Vector2[]
            {
                new Vector2(1, 0),  // Right
                new Vector2(-1, 0), // Left
                new Vector2(0, 1),  // Down
                new Vector2(0, -1)  // Up
            };

            foreach (Vector2 dir in directions)
            {
                Vector2 neighborPos = cellPos + dir;

                // Check if the position is valid
                if (IsValidPosition(neighborPos))
                {
                    Room neighborRoom = floorPlan.rooms[(int)neighborPos.x, (int)neighborPos.y];

                    // Add to adjacent rooms if it's a different room and not null
                    if (neighborRoom != null && neighborRoom != room && !adjacentRooms.Contains(neighborRoom))
                    {
                        adjacentRooms.Add(neighborRoom);
                    }
                }
            }
        }

        return adjacentRooms;
    }

    /// <summary>
    /// Checks if a position is within the bounds of the floor plan
    /// </summary>
    private bool IsValidPosition(Vector2 position)
    {
        int rows = floorPlan.rooms.GetLength(0);
        int cols = floorPlan.rooms.GetLength(1);

        return position.x >= 0 && position.x < rows &&
               position.y >= 0 && position.y < cols;
    }
}