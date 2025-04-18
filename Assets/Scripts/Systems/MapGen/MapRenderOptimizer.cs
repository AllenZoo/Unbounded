using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles activating rooms near the player to limit the load neccessary.
/// </summary>
public class MapRenderOptimizer
{
    private Dictionary<Room, GameObject> roomToPfbMap;
    private Dictionary<GameObject, Room> pfbToRoomMap;
    private Dictionary<Vector2, Room> worldToRoomMap;

    private Room startRoom;
    private FloorPlan floorPlan;
    private int borderLayer = 2;

    private HashSet<Room> activeRooms;

    public MapRenderOptimizer(Dictionary<Room, GameObject> roomToPfbMap, Dictionary<GameObject, Room> pfbToRoomMap, Dictionary<Vector2, Room> worldToRoomMap, Room startRoom, FloorPlan floorPlan)
    {
        this.roomToPfbMap = roomToPfbMap;
        this.pfbToRoomMap = pfbToRoomMap;
        this.worldToRoomMap = worldToRoomMap;
        this.startRoom = startRoom;
        this.floorPlan = floorPlan;

        Init();

        EventBinding<OnPlayerEnterRoom> playerEnterRoomBinding = new EventBinding<OnPlayerEnterRoom>(OnPlayerEnterRoomEvent);
        EventBus<OnPlayerEnterRoom>.Register(playerEnterRoomBinding);
    }

    private void OnPlayerEnterRoomEvent(OnPlayerEnterRoom room)
    {
        Debug.Log("Player entered room!");
        Room enteredRoom = pfbToRoomMap[room.roomPfb];
        HashSet<Room> roomsToLoad = GetRoomsToLoad(enteredRoom, borderLayer);
        LoadRooms(roomsToLoad);
        DisableAllButProvided(roomsToLoad);
    }

    private void Init()
    {
        // TODO: disable any active rooms not within range of player.
        HashSet<Room> roomsToLoad = GetRoomsToLoad(startRoom, borderLayer);
        LoadRooms(roomsToLoad);
        DisableAllButProvided(roomsToLoad);
    }

    /// <summary>
    /// Returns a list of loads to load.
    /// Given a starting room, get all the neighbouring rooms within 'borderLayer' distance.
    /// </summary>
    /// <param name="curRoom">room that player is in.</param>
    /// <param name="borderLayer">Distance from current room to check for neighbors</param>
    /// <returns>List of rooms to load</returns>
    private HashSet<Room> GetRoomsToLoad(Room curRoom, int borderLayer = 1)
    {
        HashSet<Room> roomsToLoad = new HashSet<Room>();
        Room[,] rooms = floorPlan.rooms;
        int gridWidth = rooms.GetLength(0);
        int gridHeight = rooms.GetLength(1);

        Vector2 curRoomPosition = curRoom.position;

        // Add the current room first
        roomsToLoad.Add(curRoom);

        Vector2 borderLeftCorner =  new Vector2(curRoomPosition.x - borderLayer, curRoomPosition.y - borderLayer);

        if (curRoomPosition.x - borderLayer < 0)
        {
            borderLeftCorner.x = 0;
        }

        if (curRoomPosition.y - borderLayer < 0)
        {
            borderLeftCorner.y = 0;
        }

        int distXToTraverse = 2 * borderLayer + (int) curRoom.size.x;
        int distYToTraverse = 2 * borderLayer + (int) curRoom.size.y;

        for (int xOffset  = 0; xOffset < distXToTraverse; xOffset++)
        {
            for (int yOffset = 0; yOffset < distYToTraverse; yOffset++)
            {
                int xPos = (int) borderLeftCorner.x + xOffset;
                int yPos = (int) borderLeftCorner.y + yOffset;

                if (xPos > gridWidth - 1) continue;
                if (yPos > gridHeight - 1) continue;

                var roomToCheck = rooms[xPos, yPos];

                if (roomsToLoad.Contains(roomToCheck)) continue;

                if (roomToCheck == null)
                {
                    // Found empty room to check. Assumes the rooms are 1x1, so position always exists. (eg. if 2x2 might be missing 3 cells in map)
                    roomToCheck = worldToRoomMap[new Vector2(xPos, yPos)];
                }
                roomsToLoad.Add(roomToCheck);
            }
        }

        // Debug.Log($"Found {roomsToLoad.Count} rooms to load.");
     
        return roomsToLoad;
    }

    private void LoadRooms(HashSet<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            roomToPfbMap[room].SetActive(true);
        }
    }

    /// <summary>
    /// Given a list of rooms, disable any other corresponding roomPfb not in list.
    /// </summary>
    /// <param name="rooms">lists to not disable</param>
    private void DisableAllButProvided(HashSet<Room> rooms)
    {
        foreach (var roomPair in roomToPfbMap)
        {
            if (!rooms.Contains(roomPair.Key))
            {
                // Should be disabled
                roomPair.Value.SetActive(false);
            }
        }
    }
}
