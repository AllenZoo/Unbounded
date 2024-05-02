using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class FloorPlanGenerator
{
    /// <summary>
    /// The floor plan is a 2D array of rooms. Each room has a position and a size.
    /// Origin is at the top left corner. (0, 0)
    /// </summary>
    protected FloorPlan floorplan;

    protected Vector2 floorplanSize = new Vector2(8, 8);
    protected int roomsToGenerate;
    protected int roomsGenerated = 0;
    protected Queue<Room> roomsToVisit = new Queue<Room>();

    /// <summary>
    /// The minimum distance of a BOSS room from the start room.
    /// </summary>
    protected int minRoomsFromStart;

    /// <summary>
    /// Probability of generating a room of a certain size.
    /// All probabilities start at 25% (25).
    /// Whenever a room of a certain size is generated, its probability is halved, and split
    /// between the other room sizes.
    /// Total probability should always be 100% (100).
    /// </summary>
    protected Dictionary<RoomSize, double> roomSizeProbMap = new Dictionary<RoomSize, double>(); 

    
    public FloorPlanGenerator(Vector2 floorPlanSize, int roomsToGenerate)
    {
        this.floorplanSize = floorPlanSize;
        this.roomsToGenerate = roomsToGenerate;
        this.minRoomsFromStart = 3;
    }

    public FloorPlanGenerator(Vector2 floorPlanSize, int roomsToGenerate, int roomsBetweenStartAndBoss)
    {
        this.floorplanSize = floorPlanSize;
        this.roomsToGenerate = roomsToGenerate;
        this.minRoomsFromStart = roomsBetweenStartAndBoss;
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
    public FloorPlan Generate()
    {
        // Generate until we get a valid floor plan.
        bool validFloorPlan = false;
        while (!validFloorPlan)
        {
            InitStartRoom();
            GenerateFloorPlan();
            validFloorPlan = AssignBossRoom();
        }
        
        // VizFloorPlan.PrintFloorPlan(floorplan.rooms);
        return floorplan;
    }

    /// <summary>
    /// Clears current floor plan and then randomly creates a start room somewhere in the floor plan. A start room is a 1x1 room.
    /// </summary>
    private void InitStartRoom()
    {
        ClearFloorPlan();
        // 1. Create a start room somewhere in the floor plan and add it to the queue.
        Vector2 randomPos = new Vector2(
            (int) Random.Range(0, floorplanSize.x - 1), 
            (int) Random.Range(0, floorplanSize.y - 1));
        Room startRoom = new Room(new Vector2(1, 1), randomPos, null, RoomType.Start);
        AddRoomToFloorPlan(startRoom);
        roomsToVisit.Enqueue(startRoom);
    }

    /// <summary>
    /// Generates floor plan.
    /// </summary>
    private void GenerateFloorPlan()
    {
        // 2. While we need to generate more rooms:
        while (roomsGenerated < roomsToGenerate) {
            // If no more rooms to visit, rerun.
            if (roomsToVisit.Count == 0)
            {
                Generate();
                break;
            }

            Room currentRoom = roomsToVisit.Dequeue();
            HashSet<Vector2> neighbours = GetNeighbouringCells(currentRoom);
            bool hasNeighbours = false;
            foreach (Vector2 neighbour in neighbours)
            {
                Room newRoom = GenerateRoom(neighbour, currentRoom);
                if (newRoom != null)
                {
                    hasNeighbours = true;
                    roomsToVisit.Enqueue(newRoom);
                    AddRoomToFloorPlan(newRoom);
                    //Debug.Log("Added room with position: " + newRoom.position);
                    roomsGenerated++;
                }

                if (roomsGenerated >= roomsToGenerate)
                {
                    break;
                }
            }    

            if (!hasNeighbours)
            {
                floorplan.deadEnds.Add(currentRoom);
                //Debug.Log("Added dead end room with position: " + currentRoom.position);
            }
        }

        // 3. Add all remaining rooms in the queue to the dead ends list.
        floorplan.deadEnds.AddRange(roomsToVisit);
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

        // ii. If we decide to create a room, get a list of possible rooms to create and pick one.
        //     (randomly selecting from 1x1, 1x2, 2x1, 2x2 sized rooms)

        List<Room> possibleRooms = GetPossibleRoomsToCreate(pos);
        if (possibleRooms.Count == 0)
        {
            return null;
        }

        // Draw a RoomSize until we get a room that is in the possibleRooms list.
        Room newRoom = null;
        RoomSize roomSizeType = RoomSize.OneByOne;
        while (newRoom == null)
        {
            roomSizeType = DrawRoomSize(roomSizeProbMap);
            newRoom = GetRoomOfRoomSize(roomSizeType, possibleRooms);
        }
        
        // Update probability map.
        UpdateProbMap(roomSizeType);

        // Debug.Log("RoomSize Type of: " + roomSizeType.ToString() + " generated!");

        newRoom.parent = parentRoom;
        return newRoom;
    }

    /// <summary>
    /// Gets random boss room from the list of dead ends.
    /// </summary>
    /// <returns>Whether Boss room was assigned properly</returns>
    private bool AssignBossRoom()
    {
        // Shuffle deadEnds list. (Where the randomization happens)
        floorplan.deadEnds = new HashSet<Room>(floorplan.deadEnds.OrderBy(x => Random.value));

        // Pick the first room that matches Boss Room criteria.
        foreach (Room deadEnd in floorplan.deadEnds)
        {
            if (deadEnd.GetDistFromStart() > minRoomsFromStart)
            {
                deadEnd.roomType = RoomType.Boss;
                return true;
            }
        }

        // No Suitable rooms found.
        return false;
    }

    /// <summary>
    /// Returns a list of possible rooms that covers the given position.
    /// Remember, the position of the room is the top left corner cell of the room.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private List<Room> GetPossibleRoomsToCreate(Vector2 position)
    {
        List<Room> possibleRooms = new List<Room>();

        // 1 option for 1x1
        possibleRooms.Add(new Room(new Vector2(1, 1), position, null));

        // 2 options for 1x2
        possibleRooms.Add(new Room(new Vector2(1, 2), position, null));
        Vector2 posOffset = new Vector2(0, -1); // Up with regards to our grid coordinate system.
        possibleRooms.Add(new Room(new Vector2(1, 2), position + posOffset, null));

        // 2 options for 2x1
        possibleRooms.Add(new Room(new Vector2(2, 1), position, null));
        posOffset = Vector2.left;
        possibleRooms.Add(new Room(new Vector2(2, 1), position + posOffset, null));

        // 4 options for 2x2
        possibleRooms.Add(new Room(new Vector2(2, 2), position, null));
        posOffset = Vector2.left;
        possibleRooms.Add(new Room(new Vector2(2, 2), position + posOffset, null));
        posOffset = new Vector2(0, -1); // Up with regards to our grid coordinate system.
        possibleRooms.Add(new Room(new Vector2(2, 2), position + posOffset, null));
        posOffset = Vector2.left + new Vector2(0, -1); // + Up with regards to our grid coordinate system.
        possibleRooms.Add(new Room(new Vector2(2, 2), position + posOffset, null));

        // Validate and filter out unsuitable rooms.
        possibleRooms = possibleRooms.FindAll(room => isEmptyRoomPos(room) && isValidRoomPos(room));
        return possibleRooms;
    }

    /// <summary>
    /// Pick out a random room of RoomSize from the prioList[ordered from high to low priority] or a singular
    /// roomSize. 
    /// If no possible rooms, return null.
    /// </summary>
    /// <returns>a random room</returns>
    private Room GetRoomOfRoomSize(RoomSize roomSize, List<Room> rooms)
    {
        List<Room> roomsOfRoomSize = rooms.FindAll(room => Room.Vector2ToRoomSize(room.size) == roomSize);
        if (roomsOfRoomSize.Count > 0)
        {
            return roomsOfRoomSize[Random.Range(0, roomsOfRoomSize.Count)];
        }
        
        return null;
    }

    /// <summary>
    /// Draws a room size based on the probability map.
    /// </summary>
    /// <param name="probMap"></param>
    /// <returns></returns>
    private RoomSize DrawRoomSize(Dictionary<RoomSize, double> probMap)
    {
        // Calculate the total probability sum
        double totalProbability = 0;
        foreach (var probability in probMap.Values)
        {
            totalProbability += probability;
        }

        // Generate a random number between 0 and the total probability sum
        double randomValue = Random.Range(0, (float) totalProbability);

        // Iterate through the dictionary and accumulate probabilities until the random value is exceeded
        double cumulativeProbability = 0;
        foreach (var kvp in probMap)
        {
            cumulativeProbability += kvp.Value;
            if (randomValue < cumulativeProbability)
            {
                return kvp.Key; // Return the RoomSize corresponding to the current entry
            }
        }

        // This point should never be reached
        throw new InvalidOperationException("No RoomSize selected. Current ProbMap: " + probMap.ToString());
    }

    /// <summary>
    /// Gets the neighbouring cells of a room.
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private HashSet<Vector2> GetNeighbouringCells(Room room)
    {
        HashSet<Vector2> neighbours = new HashSet<Vector2>();
        // Stores the individual neighbours of each cell of room. AKA. as potential neighbours.
        HashSet<Vector2> cellNeighbours = new HashSet<Vector2>();

        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
            {
                // Get all valid neighbouring cells.
                // Note room.position is the top left cell.
                Vector2 cell = new Vector2(room.position.x + x, room.position.y + y);
                cellNeighbours.AddRange(GetNeighbouringCells(cell));
            }
        }

        // Process cellNeighbours to get valid neighbours of room.
        foreach (Vector2 cell in cellNeighbours)
        {
            if (!isInRoom(room, cell) && isValidCellPos(cell))
            {
                neighbours.Add(cell);
            }
        }

        return neighbours;
    }

    /// <summary>
    /// Get the neighbouring cells of a cell. (four directions)
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private List<Vector2> GetNeighbouringCells(Vector2 cell)
    {
        List<Vector2> neighbours = new List<Vector2>();
        Vector2[] offsets = new Vector2[4] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1)};

        for (int i = 0; i < offsets.Length; i++)
        {
            if (isValidCellPos(cell, offsets[i]))
            {
                neighbours.Add(cell + offsets[i]);
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

    private bool isValidRoomPos(Room room)
    {
        for (int i = 0; i < room.size.x; i++)
        {
            for (int j = 0; j < room.size.y; j++)
            {
                if (!isValidCellPos(room.position + new Vector2(i, j)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Helper to check if cell is empty at floor plan position cell.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool isEmptyCellPos(Vector2 cell)
    {
        if (!isValidCellPos(cell))
        {
            return false;
        }
        return floorplan.rooms[(int) cell.x, (int) cell.y] == null;
    }

    /// <summary>
    /// Checks whether a given room position is empty on floor plan.
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private bool isEmptyRoomPos(Room room)
    {
        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
            {
                if (!isEmptyCellPos(new Vector2(room.position.x + x, room.position.y + y)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Checks whether cell pos is within given room.
    /// </summary>
    /// <param name="room"></param>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool isInRoom(Room room, Vector2 cell)
    {
        Vector2[] roomCells = new Vector2[(int) (room.size.x * room.size.y)];
        for (int x = 0; x < room.size.x; x++)
        {
            for (int y = 0; y < room.size.y; y++)
            {
                roomCells[(int) (x + y * room.size.x)] = new Vector2(room.position.x + x, room.position.y + y);
            }
        }


        return roomCells.Contains(cell);
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
                if (floorplan.rooms[(int)(room.position.x + i), (int)(room.position.y + j)] != null)
                {
                    Debug.LogError("Room already exists at position: " + room.position);
                    return;
                } else
                {
                    floorplan.rooms[(int)(room.position.x + i), (int)(room.position.y + j)] = room;
                }
            }
        }
    }

    /// <summary>
    /// Resets floor plan back to empty state. As a sideeffect also initializes the array.
    /// </summary>
    private void ClearFloorPlan()
    {
        floorplan = new FloorPlan((int)floorplanSize.x, (int)floorplanSize.y);
        roomsGenerated = 0;
        roomsToVisit.Clear();
        ResetProbMap();
    }

    private void ResetProbMap()
    {
        roomSizeProbMap.TryAdd(RoomSize.OneByOne, 25);
        roomSizeProbMap.TryAdd(RoomSize.OneByTwo, 25);
        roomSizeProbMap.TryAdd(RoomSize.TwoByOne, 25);
        roomSizeProbMap.TryAdd(RoomSize.TwoByTwo, 25);
    }

    /// <summary>
    /// In charge of updating the probability map.
    /// Whenever a room of a certain size is generated, its probability is halved, and split
    /// between the other room sizes.
    /// Total probability should always be 1.
    /// </summary>
    /// <param name="roomSize"></param>
    private void UpdateProbMap(RoomSize roomSize)
    {
        double probToDistributeTotal = roomSizeProbMap[roomSize] / 2;
        roomSizeProbMap[roomSize] = probToDistributeTotal;
        double probToDistributeToEach = probToDistributeTotal / (roomSizeProbMap.Count - 1);


        foreach (RoomSize key in roomSizeProbMap.Keys.ToList())
        {
            if (key != roomSize)
            {
                roomSizeProbMap[key] += probToDistributeToEach;
            }
        }
    }
}
