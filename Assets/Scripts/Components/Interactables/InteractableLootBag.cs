using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InteractableLootBag : MonoBehaviour, IInteractable
{

    // OPTIONAL: instead of reference to lootBagUI, we can reference the open button.
    [Tooltip("The system in which we want to modify the inventory data of.")]
    [SerializeField] private InventorySystem lootBagSystem;

    [Tooltip("Reference to the loot holder that contains information about the loot bag.")]
    [SerializeField] private LootHolder lootHolder;

    // [Tooltip("The inventory shared between loot bags, which is displayed on the UI.")]
    private SO_Inventory lootBagDisplayInventory;

    private void Awake()
    {
        Assert.IsNotNull(lootHolder);
    }

    private void Start()
    {
        lootBagDisplayInventory = ScriptableObject.CreateInstance<SO_Inventory>();
        lootBagDisplayInventory.items = lootHolder.GetLoot();
        lootBagDisplayInventory.slots = lootHolder.GetNumSlots();
    }

    // Show Loot
    public void Interact()
    {
        // Debug.Log("Interacted with Loot Bag: " + gameObject.transform.parent.name);
        if (lootBagSystem != null)
        {
            lootBagSystem.gameObject.SetActive(true);
            lootBagSystem.SetInventoryData(lootBagDisplayInventory);
        }
        
    }

    public void UnInteract()
    {
        if (lootBagSystem != null)
        {
            lootBagSystem.gameObject.SetActive(false);
        }
    }

    public float Priority { get; set; } = 0;
    public bool RequiresKeyPress { get; set; } = false;
    public KeyCode Key { get; set; } = KeyCode.E;
    public bool IsInteracting { get; set; } = false;

}
