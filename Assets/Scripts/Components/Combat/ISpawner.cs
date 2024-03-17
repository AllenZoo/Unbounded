using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Spawner
{
    // Spawn an object at the given position.
    public void Spawn(Vector2 pos);
}
