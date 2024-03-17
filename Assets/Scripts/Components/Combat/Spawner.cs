using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpawner

{
    /// <summary>
    /// Spawn an object at given position
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="spawn"></param>
    public void Spawn(Vector2 pos, GameObject spawn);
}
