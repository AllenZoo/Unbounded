using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootHolder : MonoBehaviour
{
    // TODO: remove serialization. Here currently for debugging
    [Header("Loot. Displayed for debugging.")]
    [SerializeField] private List<Item> loot = new List<Item>();
    [SerializeField] private int numSlots = 3;

    public void SetLoot(List<Item> loot)
    {
        this.loot = loot;
    }

    public List<Item> GetLoot()
    {
        return loot;
    }

    public int GetNumSlots()
    {
        return numSlots;
    }
}
