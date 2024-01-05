using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InteractableLootBag : MonoBehaviour, IInteractable
{

    // OPTIONAL: instead of reference to lootBagUI, we can reference the open button.
    [Tooltip("The UI object that displays the loot bag inventory. " +
        "Can be null if all we want is to modify the loog bat inventory.")]
    [SerializeField] private GameObject lootBagUI;

    [Tooltip("The inventory shared between loot bags, which is displayed on the UI.")]
    [SerializeField] private SO_Inventory lootBagDisplayInventory;

    [Tooltip("Reference to the loot holder that contains information about the loot bag.")]
    [SerializeField] private LootHolder lootHolder;

    public float Priority { get; set; } = 0;
    public bool RequiresKeyPress { get; set; } = false;
    public KeyCode Key { get; set; } = KeyCode.E;

    private void Awake()
    {
        Assert.IsNotNull(lootBagDisplayInventory);
        Assert.IsNotNull(lootHolder);
    }

    // Show Loot
    public void Interact()
    {
        Debug.Log("Interacted with Loot Bag: " + gameObject.transform.parent.name);
        if (lootBagUI != null)
        {
            lootBagUI.SetActive(true);
        }
        lootBagDisplayInventory.items = lootHolder.GetLoot();
        lootBagDisplayInventory.InvokeOnDataChange();
    }

    public void UnInteract()
    {
        if (lootBagUI != null)
        {
            lootBagUI.SetActive(false);
        }
    }
}
