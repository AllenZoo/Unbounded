using AYellowpaper.SerializedCollections;
using UnityEngine;

// Singleton that stores all inventory systems for access in other scripts.
// IE. Lootbags.
public class InventorySystemStorage : Singleton<InventorySystemStorage>
{
    [SerializedDictionary("inventory type", "inventory ref.")]
    [SerializeField] private SerializedDictionary<InventoryType, InventorySystem> inventories;

    public InventorySystem GetSystem(InventoryType type)
    {
        return inventories[type];
    }
}

public enum InventoryType
{
    None,
    Inventory,
    Equipment,
    Loot
}