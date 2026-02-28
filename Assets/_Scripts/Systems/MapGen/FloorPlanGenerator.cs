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
        this.floorplan = new FloorPlan((int)floorplanSize.x, (int)floorplanSize.y);
    }

    public FloorPlanGenerator(Vector2 floorPlanSize, int roomsToGenerate, int roomsBetweenStartAndBoss)
    {
        this.floorplanSize = floorPlanSize;
        this.roomsToGenerate = roomsToGenerate;
        this.minRoomsFromStart = roomsBetweenStartAndBoss;
        this.floorplan = new FloorPlan((int)floorplanSize.x, (int)floorplanSize.y); 
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
    /// 3. Once we generated enough rooms, sift through dead-end rooms to find/assign a boss room
    ///     a. a dead-end room is a valid boss room if it is:
    ///         i. a 2x2 room
    ///         ii. some non-2x2 room that has enough space to turn into a 2x2 room.
    /// </summary>
    public FloorPlan Generate()
    {
        // Generate until we get a valid floor plan.
        bool validFloorPlan = false;
        while (!validFloorPlan)
        {
            InitStartRoom();
            GenerateFloorPlan();
            AssignBossRoom();

            var floorPlanValidator = new FloorPlanValidator(floorplan);
            validFloorPlan = floorPlanValidator.ValidateFloorPlan();

            if (!validFloorPlan)
            {
                Debug.Log("Got invalid floorplan. Need to regen floorplan!");
                ClearFloorPlan();
            }
        }

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
        floorplan.AddRoom(startRoom);
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
            foreach (Vector2 neighbour in neighbours)
            {
                Room newRoom = GenerateRoom(neighbour, currentRoom);
                if (newRoom != null)
                {
                    roomsToVisit.Enqueue(newRoom);
                    floorplan.AddRoom(newRoom);
                    //Debug.Log("Added room with position: " + newRoom.position);
                    roomsGenerated++;
                }

                if (roomsGenerated >= roomsToGenerate)
                {
                    break;
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

        // ii. If we decide to create a room, get a list of possible rooms to create and pick one.
        //     (randomly selecting from 1x1, 1x2, 2x1, 2x2 sized rooms)

        List<Room> possibleRooms = GetPossibleRoomsToCreate(pos, new List<RoomSize>() { RoomSize.OneByOne, RoomSize.TwoByTwo, RoomSize.TwoByOne, RoomSize.OneByTwo});
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
    /// Gets random boss room from the list of dead ends. Boss room has to be 2x2.
    /// </summary>
    /// <returns>Whether Boss room was assigned properly</returns>
    private bool AssignBossRoom()
    {
        // Shuffle deadEnds list. (Where the randomization happens)
        // Note: We create a new set, since we might modify the deadEnds set below during the loop.
        HashSet<Room> deadEnds = new HashSet<Room>(floorplan.DeadEnds.OrderBy(x => Random.value));
        Debug.Log("Assigning Boss Room!");

        // Pick the first room that matches Boss Room criteria.
        foreach (Room deadEnd in deadEnds)
        {
            if (deadEnd.GetDistFromStart() > minRoomsFromStart)
            {
                // Check if deadEnd is 2x2 or can transform into 2x2 room.
                // If it's not 2x2 but can transform randomly pick the an option.

                List<Room> possible2x2 = GetRoomTransformOptions(deadEnd, RoomSize.TwoByTwo);
                if (possible2x2.Count == 0) continue;

                int index = Random.Range(0, possible2x2.Count);
                Room newRoom = possible2x2[index];
                newRoom.roomType = RoomType.Boss;

                floorplan.SwapRoom(deadEnd, newRoom);

                return true;
            }
        }

        // No Suitable rooms found.
        return false;
    }


    /// <summary>
    /// Given a room and desired room size to transform into, return a list of new rooms that would fit in map and not overlap with other rooms.
    /// 
    /// If provided room has a roomSize equal to given roomSize, returns a list containing given room.
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private List<Room> GetRoomTransformOptions(Room room, RoomSize roomSize)
    {
        List<Room> rooms;
        // TODO: apparently room.roomSize has some bug? idk check if this if statement works. CTRL + F Room.cs and search for "roomSize" to see what I mean.
        if (room.roomSize == roomSize) return new List<Room>() { room };

        // Temporarily remove room so that it is temporarily not *exsisting* for algo that checks if cell empty, etc.
        floorplan.RemoveRoom(room);
        rooms = GetPossibleRoomsToCreate(room.position, new List<RoomSize>() { roomSize });
        floorplan.AddRoom(room); // Add room back.

        return rooms;
    }
 

    /// <summary>
    /// Returns a list of possible rooms that covers the given position, and is one of the provided room sizes requested..
    /// Remember, the position of the room is the top left corner cell of the room.
    /// </summary>
    /// <param name="position"></param>
    /// <returns>a list of possible rooms that cover given position.</returns>
    private List<Room> GetPossibleRoomsToCreate(Vector2 position, List<RoomSize> roomSizes)
    {
        List<Room> possibleRooms = new List<Room>();
        Vector2 posOffset;

        // 1 option for 1x1
        if (roomSizes.Contains(RoomSize.OneByOne))
        {
            possibleRooms.Add(new Room(new Vector2(1, 1), position, null));
        }


        // 2 options for 1x2
        if (roomSizes.Contains(RoomSize.OneByTwo))
        {
            possibleRooms.Add(new Room(new Vector2(1, 2), position, null));
            posOffset = new Vector2(0, -1); // Up with regards to our grid coordinate system.
            possibleRooms.Add(new Room(new Vector2(1, 2), position + posOffset, null));
        }


        // 2 options for 2x1
        if (roomSizes.Contains(RoomSize.TwoByOne))
        {
            possibleRooms.Add(new Room(new Vector2(2, 1), position, null));
            posOffset = Vector2.left;
            possibleRooms.Add(new Room(new Vector2(2, 1), position + posOffset, null));
        }


        // 4 options for 2x2
        if (roomSizes.Contains(RoomSize.TwoByTwo))
        {
            possibleRooms.Add(new Room(new Vector2(2, 2), position, null));
            posOffset = Vector2.left;
            possibleRooms.Add(new Room(new Vector2(2, 2), position + posOffset, null));
            posOffset = new Vector2(0, -1); // Up with regards to our grid coordinate system.
            possibleRooms.Add(new Room(new Vector2(2, 2), position + posOffset, null));
            posOffset = Vector2.left + new Vector2(0, -1); // + Up with regards to our grid coordinate system.
            possibleRooms.Add(new Room(new Vector2(2, 2), position + posOffset, null));
        }
            

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
    /// <returns>a randomly drawn room size.</returns>
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
    /// <returns>a set of neighbouring cells</returns>
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
    /// Resets floor plan back to empty state. As a sideeffect also initializes the array.
    /// </summary>
    private void ClearFloorPlan()
    {
        Debug.Log("Resetting Floor Plan!");

        floorplan.Reset();
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
