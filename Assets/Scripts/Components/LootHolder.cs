using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootHolder : MonoBehaviour
{
    // TODO: remove serialization. Here currently for debugging
    [Header("Loot. Displayed for debugging.")]
    [SerializeField] private List<SO_Item> loot = new List<SO_Item>();
    [SerializeField] private int numSlots = 3;

    public void SetLoot(List<SO_Item> loot)
    {
        this.loot = loot;
    }

    public List<SO_Item> GetLoot()
    {
        return loot;
    }

    public int GetNumSlots()
    {
        return numSlots;
    }
}
