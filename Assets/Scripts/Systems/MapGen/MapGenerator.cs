using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator: MonoBehaviour {

    // TODO-OPT: Refactor these lists into a dictionary for easier access. (Refer to RoomDoorHandler.cs)
    [Header("Room Prefabs")]
    [SerializeField] private List<PfbRoomSizeTypeTuple> normalRoomPfbs = new List<PfbRoomSizeTypeTuple>();
    [SerializeField] private List<PfbRoomSizeTypeTuple> startRoomPfbs = new List<PfbRoomSizeTypeTuple>();
    [SerializeField] private List<PfbRoomSizeTypeTuple> bossRoomPfbs = new List<PfbRoomSizeTypeTuple>();

    [Header("Map Gen Settings")]
    [SerializeField] private Vector2 mapSize = new Vector2(8, 8);
    [Tooltip("Number of rooms to generate. (excluding start room)")]
    [SerializeField] private int roomsToGenerate = 12;
    [Tooltip("Number of rooms between start and boss room. (excluding the start and boss room)")]
    [SerializeField] private int roomsBetweenStartAndBoss = 3;

    /// <summary>
    /// For placing the rooms in the world.
    /// </summary>
    /// 
    [Tooltip("Size of the room in world units. (width, height)")]
    [SerializeField] private Vector2 roomSizeWorldUnits;

    private Dictionary<Room, GameObject> roomToPfbMap;
    private FloorPlanGenerator floorPlanGenerator;
    private GameObject baseMap;

    // Reference to be passed through the OnMapGenerated event.
    private GameObject startRoomPfb;

    private void Start()
    {
        floorPlanGenerator = new FloorPlanGenerator(mapSize, roomsToGenerate, roomsBetweenStartAndBoss);
        roomToPfbMap = new Dictionary<Room, GameObject>();
    }

    /// <summary>
    /// Generate a new floor plan.
    /// Traverse through the rooms starting at dead ends and then linking them together.
    /// </summary>
    public void GenerateMap()
    {
        if (baseMap != null)
        {
            Destroy(baseMap);
        }
        baseMap = new GameObject("BaseMap");
        FloorPlan floorPlan = floorPlanGenerator.Generate();
        VizFloorPlan.PrintFloorPlan(floorPlan.rooms, floorPlan);
        InstantiateMap(floorPlan);

        EventBus<OnMapGeneratedEvent>.Call(new OnMapGeneratedEvent{
            startRoomPfb = this.startRoomPfb
        });
    }

    /// <summary>
    /// Traverse through the floor plan and instantiate each room seperately.
    /// 
    /// </summary>
    protected void InstantiateMap(FloorPlan floorPlan)
    {
        InstantiateRooms(floorPlan.deadEnds);
        InstantiateCorridors(floorPlan.deadEnds);
    }

    protected void InstantiateRooms(HashSet<Room> deadEnds)
    {
        // TODO-OPT: essentially dupe of InstantiateCorridors. Could refactor for 
        // optimal code. Split to keep modularity of room and corridor creation.
        Queue<Room> toVisit = new Queue<Room>(deadEnds);
        HashSet<Room> visited = new HashSet<Room>();
        while (toVisit.Count > 0)
        {
            Room current = toVisit.Dequeue();
            if (visited.Contains(current))
            {
                continue;
            }
            visited.Add(current);

            InstantiateRoom(current);
            if (current.parent != null)
            {
                toVisit.Enqueue(current.parent);
            }
        }
    }

    /// <summary>
    /// Pick a random suitable room prefab and instantiate it.
    /// </summary>
    /// <param name="room"></param>
    protected void InstantiateRoom(Room room)
    {
        GameObject roomPfb = GetSuitableRoomPfb(room.roomType, room.roomSize);
        if (roomPfb != null)
        {
            // Note: Y is inverted in Unity Transform system compared to our grid system. Thus we flip the y coordinate.
            Vector3 roomPos = new Vector3(room.position.x * roomSizeWorldUnits.x, -room.position.y * roomSizeWorldUnits.y, 0);

            GameObject roomObj = Instantiate(roomPfb, roomPos, Quaternion.identity);
            roomObj.transform.SetParent(baseMap.transform);
            roomToPfbMap.Add(room, roomObj);

            // Check if is start room. If it is, store ref.
            if (room.roomType == RoomType.Start)
            {
                startRoomPfb = roomObj;
            }

            // Debug.Log("Instantiated roomPfb at " + room.position);
        } else
        {
            Debug.LogError("Error generating Map: No suitable roomPfb found!");
        }
    }

    protected GameObject GetSuitableRoomPfb(RoomType roomType, RoomSize roomSize)
    {
        switch (roomType)
        {
            case RoomType.Normal:
                return GetSuitableRoomPfb(roomSize, normalRoomPfbs);
            case RoomType.Start:
                return GetSuitableRoomPfb(roomSize, startRoomPfbs);
            case RoomType.Boss:
                return GetSuitableRoomPfb(roomSize, bossRoomPfbs);
        }   
        return null;
    }

    protected GameObject GetSuitableRoomPfb(RoomSize roomSize, List<PfbRoomSizeTypeTuple> roomPfbs)
    {
        // Shuffle List for randomization
        Shuffle(roomPfbs);
        foreach (PfbRoomSizeTypeTuple tuple in roomPfbs)
        {
            if (tuple.size == roomSize)
            {
                return tuple.pfb;
            }
        }

        // No suitable room found return null.
        return null;
    }

    protected void InstantiateCorridors(HashSet<Room> deadEnds)
    {
        Queue<Room> toVisit = new Queue<Room>(deadEnds);
        HashSet<Room> visited = new HashSet<Room>();
        while (toVisit.Count > 0)
        {
            Room current = toVisit.Dequeue();
            if (visited.Contains(current))
            {
                continue;
            }
            visited.Add(current);

            // Don't need to instantiate corridor for start room.
            if (current.parent != null)
            {
                InstantiateCorridor(current, current.parent);
                toVisit.Enqueue(current.parent);
            }
        }
    }

    /// <summary>
    /// Create a corridor between the two rooms.
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    protected void InstantiateCorridor(Room room1, Room room2)
    {
        if (!roomToPfbMap.ContainsKey(room1) || !roomToPfbMap.ContainsKey(room2))
        {
            // Map is missing a room key. ERror
            Debug.LogError("Error generating Map: Room key missing in roomToPfbMap!");
            return;
        }

        GameObject room1Pfb = roomToPfbMap[room1];
        GameObject room2Pfb = roomToPfbMap[room2];

        RoomDoorHandler rdh1 = room1Pfb.GetComponent<RoomDoorHandler>();
        RoomDoorHandler rdh2 = room2Pfb.GetComponent<RoomDoorHandler>();
        if (rdh1 == null || rdh2 == null)
        {
            // Room prefab is missing a RoomDoorHandler component. Error
            Debug.LogError("Error generating Map: Room prefab missing RoomDoorHandler component!");
            return;
        }

        // 1. Find possible door creations
        List<Tuple<RoomPosTuple, RoomPosTuple>> neighbourPairs = GetNeighbours(room1, room2);

        // 2. Pick one.
        Tuple<RoomPosTuple, RoomPosTuple> doorPair = neighbourPairs[UnityEngine.Random.Range(0, neighbourPairs.Count)];

        // 3. Create a corridor (unlock doors) between the two rooms.
        Vector2 room1CellPos = doorPair.Item1.cellPos;
        Vector2 room2CellPos = doorPair.Item2.cellPos;

        Vector2 room1Offset = room1CellPos - doorPair.Item1.room.position;
        Vector2 room2Offset = room2CellPos - doorPair.Item2.room.position;

        CellDir dir1 = GetCellDir(room1CellPos, room2CellPos);
        CellDir dir2 = GetCellDir(room2CellPos, room1CellPos);

        //Debug.Log("Opening Door between " + room1.position + " and " + room2.position + " at " 
        //    + room1CellPos + " and " + room2CellPos + " with offsets " 
        //    + room1Offset + " and " + room2Offset + " and dirs " 
        //    + dir1 + " and " + dir2 + " respectively.");

        rdh1.OpenDoor(room1Offset, dir1);
        rdh2.OpenDoor(room2Offset, dir2);
    }


    /// <summary>
    /// Get all possible neighbour pairs between two rooms. Order of tuple elements is guranteed <room1, room2>.
    /// </summary>
    /// <param name="room1"></param>
    /// <param name="room2"></param>
    /// <returns></returns>
    protected List<Tuple<RoomPosTuple, RoomPosTuple>> GetNeighbours(Room room1, Room room2)
    {
        List<Tuple<RoomPosTuple, RoomPosTuple>> neighbours = new List<Tuple<RoomPosTuple, RoomPosTuple>>();

        List<Vector2> room1CellPos = room1.GetCellPositions();
        List<Vector2> room2CellPos = room2.GetCellPositions();

        // TODO-OPT: see if we can optimize this.
        foreach (Vector2 c1 in room1CellPos)
        {
            foreach (Vector2 c2 in room2CellPos)
            {
                if (AreNeighbours(c1, c2))
                {
                    neighbours.Add(new Tuple<RoomPosTuple, RoomPosTuple>(new RoomPosTuple(room1, c1), new RoomPosTuple(room2, c2)));
                }
            }
        }

        return neighbours;
    }

    /// <summary>
    /// Checks if two cell positions are neighbours.
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    protected bool AreNeighbours(Vector2 c1, Vector2 c2)
    {
        // Checks if the two cells are neighbours.
        return Math.Abs((c1 - c2).magnitude) == 1;
    }

    /// <summary>
    /// Cell dir from c1 to c2
    /// </summary>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <returns></returns>
    protected CellDir GetCellDir(Vector2 c1, Vector2 c2)
    {
        Vector2 diff = c2 - c1;
        if (diff.x == 1)
        {
            return CellDir.Right;
        }
        if (diff.x == -1)
        {
            return CellDir.Left;
        }
        if (diff.y == 1)
        {
            return CellDir.Down; // Flipped because of our Coord System
        }
        if (diff.y == -1)
        {
            return CellDir.Up; // Flipped because of our Coord System
        }

        Debug.LogError("Should not have gotten here!");
        return CellDir.Up;
    }


    static void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

[Serializable]
public class PfbRoomSizeTypeTuple
{
    public GameObject pfb;
    public RoomSize size;
    public RoomType type;
}

public class RoomPosTuple
{
    public Room room;
    public Vector2 cellPos;

    public RoomPosTuple(Room room, Vector2 pos)
    {
        this.room = room;
        this.cellPos = pos;
    }
}
