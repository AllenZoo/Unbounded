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

    /// <summary>
    /// Represents the position of the room in the grid. If room uses multiple cells, this is the top left cell.
    /// </summary>
    public Vector2 position;

    /// <summary>
    /// First room to connect to this room. If NULL = Start Room.
    /// </summary>
    public Room parent;

    public Room(Vector2 size, Vector2 position, Room parent)
    {
        this.size = size;
        this.position = position;
        this.parent = parent;
    }

    public override string ToString()
    {
        return "{[Room] Size: " + size + ", Position: " + position + "}";
    }

    /// <summary>
    /// Returns the position taken up by the room.
    /// </summary>
    /// <returns></returns>
    //public Vector2[] GetPositions();
    //public void SetPosition(Vector2[] position);

    //// [Grid Pos, Door Pos Dir]
    //public Tuple<Vector2, Vector2> GetDoorPositions();

}
