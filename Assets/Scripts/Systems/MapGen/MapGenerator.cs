using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator: MonoBehaviour {
    [Header("Room Prefabs")]
    [SerializeField] private List<PfbRoomSizeTypeTuple> normalRoomPfbs = new List<PfbRoomSizeTypeTuple>();
    [SerializeField] private List<PfbRoomSizeTypeTuple> startRoomPfbs = new List<PfbRoomSizeTypeTuple>();
    [SerializeField] private List<PfbRoomSizeTypeTuple> bossRoomPfbs = new List<PfbRoomSizeTypeTuple>();
    [SerializeField] private GameObject corridorPfb;

    [Header("Map Gen Settings")]
    [SerializeField] private Vector2 mapSize = new Vector2(8, 8);
    [Tooltip("Number of rooms to generate. (excluding start room)")]
    [SerializeField] private int roomsToGenerate = 12;

    
    // [SerializeField] private Vector2 roomPadding;

    /// <summary>
    /// For placing the rooms in the world.
    /// </summary>
    [SerializeField] private Vector2 roomSizeWorldUnits;

    private FloorPlanGenerator floorPlanGenerator;
    private GameObject baseMap;

    private void Start()
    {
        floorPlanGenerator = new FloorPlanGenerator(mapSize, roomsToGenerate);
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

        // Instantiate(baseMap);
    }

    /// <summary>
    /// Traverse through the floor plan and instantiate each room seperately.
    /// 
    /// </summary>
    public void InstantiateMap(FloorPlan floorPlan)
    {
        InstantiateRooms(floorPlan.deadEnds);
        InstantiateCorridors(floorPlan.deadEnds);
    }

    public void InstantiateRooms(HashSet<Room> deadEnds)
    {
        // TODO: essentially dupe of InstantiateCorridors. Could refactor for 
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

            // TODO: handle start room being instantiated multiple times this way.
            InstantiateRoom(current);
            if (current.parent != null)
            {
                toVisit.Enqueue(current.parent);
            }
        }
    }

    // TODO:
    /// <summary>
    /// Pick a random suitable room prefab and instantiate it.
    /// </summary>
    /// <param name="room"></param>
    public void InstantiateRoom(Room room)
    {
        GameObject roomPfb = GetSuitableRoomPfb(room.roomType, room.roomSize);
        if (roomPfb != null)
        {
            // Note: Y is inverted in Unity Transform system compared to our grid system. Thus we flip the y coordinate.
            Vector3 roomPos = new Vector3(room.position.x * roomSizeWorldUnits.x, -room.position.y * roomSizeWorldUnits.y, 0);

            GameObject roomObj = Instantiate(roomPfb, roomPos, Quaternion.identity);
            roomObj.transform.SetParent(baseMap.transform);
            Debug.Log("Instantiated roomPfb at " + room.position);
            // Debug.Log("instantiated room");
        } else
        {
            Debug.LogError("Error generating Map: No suitable roomPfb found!");
        }
    }

    public GameObject GetSuitableRoomPfb(RoomType roomType, RoomSize roomSize)
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

    public GameObject GetSuitableRoomPfb(RoomSize roomSize, List<PfbRoomSizeTypeTuple> roomPfbs)
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

    public void InstantiateCorridors(HashSet<Room> deadEnds)
    {
        Queue<Room> toVisit = new Queue<Room>(deadEnds);
        while (toVisit.Count > 0)
        {
            Room current = toVisit.Dequeue();
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
    public void InstantiateCorridor(Room room1, Room room2)
    {

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
