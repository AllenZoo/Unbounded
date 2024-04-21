using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    /// <summary>
    /// Represents the size of the room width by height.
    /// </summary>
    public Vector2 size;

    // TODO: Flagged for potential bugs
    public RoomSize roomSize { get { return Vector2ToRoomSize(size); } }

    /// <summary>
    /// Represents the position of the room in the grid. If room uses multiple cells, this is the top left cell.
    /// </summary>
    public Vector2 position;

    /// <summary>
    /// First room to connect to this room. If NULL = Start Room.
    /// </summary>
    public Room parent;

    /// <summary>
    /// Type of room.
    /// </summary>
    public RoomType roomType;

    public Room(Vector2 size, Vector2 position, Room parent, RoomType roomType)
    {
        this.size = size;
        this.position = position;
        this.parent = parent;
        this.roomType = roomType;
    }

    /// <summary>
    /// Default room.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="position"></param>
    /// <param name="parent"></param>
    public Room(Vector2 size, Vector2 position, Room parent)
    {
        this.size = size;
        this.position = position;
        this.parent = parent;
        this.roomType = RoomType.Normal;
    }

    public override string ToString()
    {
        return "{[Room] Size: " + size + ", Position: " + position + "}";
    }

    public int GetDistFromStart()
    {
        if (parent == null)
        {
            return 0;
        }
        else
        {
            return 1 + parent.GetDistFromStart();
        }
    }

    public static RoomSize Vector2ToRoomSize(Vector2 size)
    {
        if (size == new Vector2(1, 1))
            return RoomSize.OneByOne;
        else if (size == new Vector2(1, 2))
            return RoomSize.OneByTwo;
        else if (size == new Vector2(2, 1))
            return RoomSize.TwoByOne;
        else if (size == new Vector2(2, 2))
            return RoomSize.TwoByTwo;
        else
            return RoomSize.OneByOne;
    }
}

public enum RoomType
{
    Start,
    Normal,
    Boss
}

public enum RoomSize
{
    OneByOne,
    OneByTwo,
    TwoByOne,
    TwoByTwo,
}

/// <summary>
/// For representation of boss rooms.
/// </summary>
//public class BossRoom: Room
//{
//    public BossRoom(Vector2 size, Vector2 position, Room parent) : base(size, position, parent)
//    {
//    }

//    public BossRoom(Room room) : base(room)
//    {
//        this.size = room.size;
//        this.position = room.position;
//        this.parent = room.parent;
//    }
//}