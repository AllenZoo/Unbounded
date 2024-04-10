using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room1x1 : Room
{
    protected Vector2[] position;

    public Vector2[] GetPositions()
    {
        return position;
    }

    public void SetPosition(Vector2[] position)
    {
        throw new NotImplementedException();
    }

    public Tuple<Vector2, Vector2> GetDoorPositions()
    {
        throw new NotImplementedException();
    }
}
