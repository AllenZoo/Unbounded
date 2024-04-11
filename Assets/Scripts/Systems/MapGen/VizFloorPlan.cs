using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that visualizes the floor plan of map via print.
/// </summary>
public class VizFloorPlan : MonoBehaviour
{
    public static void PrintFloorPlan(Room[] floorPlan)
    {
        string floorPlanString = "";
        for (int i = 0; i < floorPlan.GetLength(0); i++)
        {
            for (int j = 0; j < floorPlan.GetLength(1); j++)
            {
                floorPlanString += floorPlan[i, j] + " ";
            }
            floorPlanString += "\n";
        }
        Debug.Log(floorPlanString);
    }
}
