using AYellowpaper.SerializedCollections;
using System;
using UnityEngine;

/// <summary>
/// Handles opening and closing doors in the room.
/// </summary>
public class RoomDoorHandler : MonoBehaviour
{
    [SerializedDictionary]
    [SerializeField] private SerializedDictionary<DoorObjKey, GameObject> doorDict = new SerializedDictionary<DoorObjKey, GameObject>();

    /// <summary>
    /// Opens the corridor by opening/removing the door.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    public void OpenDoor(Vector2 pos, CellDir dir)
    {
        DoorObjKey key = new DoorObjKey(pos, dir);
        if (doorDict.ContainsKey(key))
        {
            doorDict[key].SetActive(false);
        }
    }

    /// <summary>
    /// Closes the corridor by closing/adding the door.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="dir"></param>
    public void CloseDoor(Vector2 pos, CellDir dir)
    {
        DoorObjKey key = new DoorObjKey(pos, dir);
        if (doorDict.ContainsKey(key))
        {
            doorDict[key].SetActive(true);
        }
    }

    /// <summary>
    /// Closes all doors in the room.
    /// </summary>
    public void CloseAllDoors()
    {
        foreach (GameObject door in doorDict.Values)
        {
            door.SetActive(true);
        }
    }
}

[Serializable]
public class DoorObjKey
{
    public Vector2 position;
    public CellDir direction;

    public DoorObjKey(Vector2 pos, CellDir dir)
    {
        position = pos;
        direction = dir;
    }

    // Overwrite Equals and Hash
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        DoorObjKey key = (DoorObjKey)obj;
        return position == key.position && direction == key.direction;
    }

    public override int GetHashCode()
    {
        return position.GetHashCode() ^ direction.GetHashCode();
    }
}


public enum CellDir
{
    Up,
    Down,
    Left,
    Right
}
