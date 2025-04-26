using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Class that tracks the spawn info of an attack. eg. position of mouse, rotation of spawn.
public class AttackSpawnInfo
{
    public Vector3 mousePosition;

    public AttackSpawnInfo(Vector2 mousePos)
    {
        this.mousePosition = mousePos;
    }
}
