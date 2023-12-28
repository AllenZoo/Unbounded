using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that holds data for inventory mouse hoverers.
public class ItemHoverer : MonoBehaviour
{

    [SerializeField] private SO_Item data;
    [SerializeField] private int slotIndex;
    
    public void SetData(SO_Item itemData)
    {
        data = itemData;
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    public SO_Item GetData()
    {
        return data;
    }

    public int GetSlotIndex()
    {
        return slotIndex;
    }
}
