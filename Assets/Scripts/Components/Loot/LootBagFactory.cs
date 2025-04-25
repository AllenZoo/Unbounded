using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Script for creating loot bag pfbs into the world.
public class LootBagFactory : Singleton<LootBagFactory>
{
    // [Tooltip("The prefab of the loot bag. Should contain sprite rendering the loot bag.")]
    [SerializeField] private GameObject lootBagPrefab;

    protected override void Awake()
    {
        base.Awake();
        // Assert properties of lootBagPrefab hold.
        // 1. has a LootHolder component
        // 2. has a SpriteRenderer component
        // 3. has a Collider2D component (in parent or child)
        // 4. has an IInteractable component (in parent or child)
        Assert.IsNotNull(lootBagPrefab, "Loot bag prefab should not be null!");
        Assert.IsNotNull(lootBagPrefab.GetComponent<LootHolder>(), "Loot bag prefab should have a LootHolder component!");
        Assert.IsNotNull(lootBagPrefab.GetComponent<SpriteRenderer>(), "Loot bag prefab should have a SpriteRenderer component!");
        Assert.IsTrue(lootBagPrefab.GetComponent<Collider2D>() != null
            || lootBagPrefab.GetComponentInChildren<Collider2D>() != null,
            "Loot bag prefab should have a Collider2D component in either root or child!");
        Assert.IsTrue(lootBagPrefab.GetComponent<IInteractable>() != null
            || lootBagPrefab.GetComponentInChildren<IInteractable>() != null,
            "Loot bag prefab should have an IInteractable component in either root or child!");
    }

    /// <summary>
    /// Creates a loot bag at the given position with the given items.
    /// </summary>
    /// <param name="position">position to spawn loot bag.</param>
    /// <param name="items">items to put inside loot bag.</param>
    /// <returns>the created loot bag object.</returns>
    public GameObject CreateLootBag(Vector3 position, List<Item> items)
    {
        // Spawn loot bag at position.
        GameObject lootBag = Instantiate(lootBagPrefab, position, Quaternion.identity);

        // Set loot items in loot bag.
        LootHolder lootHolder = lootBag.GetComponent<LootHolder>();
        lootHolder.SetLoot(items);

        return lootBag;
    }
}
