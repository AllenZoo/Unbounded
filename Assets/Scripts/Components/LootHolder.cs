using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootHolder : MonoBehaviour
{
    // TODO: remove serialization. Here currently for debugging
    [Header("Loot. Displayed for debugging.")]
    [SerializeField] private List<SO_Item> loot = new List<SO_Item>();

    public void SetLoot(List<SO_Item> loot)
    {
        this.loot = loot;
    }

    public List<SO_Item> GetLoot()
    {
        return loot;
    }
}
