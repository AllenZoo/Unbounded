using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that visualizes the floor plan of map via print.
/// </summary>
public class VizFloorPlan : MonoBehaviour
{
    public static void PrintFloorPlan(Room[,] floorPlan, FloorPlan fp)
    {
        
        string floorPlanString = "";
        Dictionary<Room, string> roomStrMap = new Dictionary<Room, string>();
        int roomCount = 1;

        for (int j = 0; j < floorPlan.GetLength(0); j++)
        {
            for (int i = 0; i < floorPlan.GetLength(1); i++)
            {
                if (floorPlan[i, j] == null)
                    floorPlanString += " N";//floorPlan[i, j] + " ";
                else if (floorPlan[i, j].parent == null)
                    floorPlanString += " S";//floorPlan[i, j] + " ";
                else if (floorPlan[i, j].roomType == RoomType.Boss)
                {
                    floorPlanString += " B";
                }
                else if (floorPlan[i, j].parent != null)
                {
                    if (!roomStrMap.ContainsKey(floorPlan[i, j]))
                    {
                        roomStrMap.Add(floorPlan[i, j], (roomCount < 10 ? " " : "") + roomCount);
                        roomCount++;
                    }
                        
                    floorPlanString += roomStrMap[floorPlan[i, j]];
                }
                    

                floorPlanString += ", ";
            }
            floorPlanString += "\n";
        }
        Debug.Log(floorPlanString);
    }
}
