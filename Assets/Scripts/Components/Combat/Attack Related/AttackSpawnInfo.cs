using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Class that tracks the spawn info of an attack. eg. position of mouse, rotation of spawn.
public class AttackSpawnInfo
{
    public Vector3 targetPosition; // aka. mouse position for player attacks.

    public AttackSpawnInfo(Vector2 mousePos)
    {
        this.targetPosition = mousePos;
    }
}
