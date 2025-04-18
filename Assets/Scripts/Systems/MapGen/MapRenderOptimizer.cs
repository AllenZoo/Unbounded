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
    private Room startRoom;
    private FloorPlan floorPlan;
    private int borderLayer = 2;

    private HashSet<Room> activeRooms;

    public MapRenderOptimizer(Dictionary<Room, GameObject> roomToPfbMap, Dictionary<GameObject, Room> pfbToRoomMap, Room startRoom)
    {
        this.roomToPfbMap = roomToPfbMap;
        this.pfbToRoomMap = pfbToRoomMap;

        EventBinding<OnPlayerEnterRoom> playerEnterRoomBinding = new EventBinding<OnPlayerEnterRoom>(OnPlayerEnterRoomEvent);
        EventBus<OnPlayerEnterRoom>.Register(playerEnterRoomBinding);
    }

    private void OnPlayerEnterRoomEvent(OnPlayerEnterRoom room)
    {
        Debug.Log("Player entered room!");
        Room enteredRoom = pfbToRoomMap[room.roomPfb];
    }

    private void Init()
    {
        // TODO: disable any active rooms not within range of player.

    }

    /// <summary>
    /// Returns a list of loads to load
    /// </summary>
    /// <param name="curRoom">room that player is in.</param>
    /// <returns></returns>
    private List<Room> GetRoomsToLoad(Room curRoom)
    {
        List<Room> roomsToLoad = new List<Room>();

        Vector2 roomPosition = curRoom.position;
        Room[,] rooms = floorPlan.rooms;
        var a = rooms[(int) roomPosition.x, (int) roomPosition.y];


        return roomsToLoad;
    }
}
