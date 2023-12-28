using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory/Inventory")]
public class SO_Inventory : ScriptableObject
{
    public int slots = 9;
    public List<SO_Item> items = new List<SO_Item>();
}
