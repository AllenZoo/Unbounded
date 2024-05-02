using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LootDropperOnDeath : MonoBehaviour, LootDropper
{
    [SerializeField] LocalEventHandler localEventHandler;
    [SerializeField] private DropRates dropRates;
    [Tooltip("The maximum number of items that can be dropped. Counts as attempts in running the drop lottery.")]
    [SerializeField] private int maxItems = 3;

    private void Awake()
    {
        Assert.IsNotNull(dropRates, "LootDropperOnDeath requires a DropRates reference.");

        if (localEventHandler == null)
        {
            localEventHandler = GetComponentInParent<LocalEventHandler>();
            if (localEventHandler == null)
            {
                Debug.LogError("LocalEventHandler unassigned and not found in parent for object [" + gameObject +
                                       "] with root object [" + gameObject.transform.root.name + "] for LootDropperOnDeath.cs");
            }
        }
    }

    private void Start()
    {
        LocalEventBinding<OnDeathEvent> onDeathBinding = new LocalEventBinding<OnDeathEvent>(DropLoot);
        localEventHandler.Register(onDeathBinding);
    }

    private void DropLoot(OnDeathEvent e)
    {
        // Get the loot to drop
        List<Item> lootDrop = DropRateCalculator.GetItemsFromDropRate(dropRates, maxItems);

        // Check if lootdrop is empty (all null items).
        bool isEmpty = true;
        foreach (Item item in lootDrop)
        {
            if (item != null)
            {
                isEmpty = false;
                break;
            }
        }
        // Loot drop is empty, return.
        if (isEmpty)
        {
            return;
        }

        // Create new loot bag with loot drops inside.
        LootBagFactory.Instance.CreateLootBag(transform.position, lootDrop);
    }
}
