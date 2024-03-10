using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO: refactor logic. Should not create SO_Inventory.
public class InteractableLootBag : MonoBehaviour, IInteractable
{

    #region IInteractable fields
    public float Priority { get; set; } = 0;
    public bool RequiresKeyPress { get; set; } = false;
    public KeyCode Key { get; set; } = KeyCode.E;
    public bool IsInteracting { get; set; } = false;
    #endregion\


    [Tooltip("Reference to the loot holder that contains information about the loot bag.")]
    [SerializeField] private LootHolder lootHolder;

    [SerializeField] private GameObject parentObject;

    // The system in which we want to modify the inventory data of.
    private InventorySystem lootBagSystem;

    // [Tooltip("The inventory shared between loot bags, which is displayed on the UI.")]
    private Inventory lootBagDisplayInventory;

    private void Awake()
    {
        Assert.IsNotNull(lootHolder);
        Assert.IsNotNull(parentObject);
    }

    private void Start()
    {
        SO_Inventory inventoryData = ScriptableObject.CreateInstance<SO_Inventory>();
        inventoryData.items = lootHolder.GetLoot();
        inventoryData.slots = lootHolder.GetNumSlots();

        lootBagDisplayInventory = new Inventory(inventoryData);
        lootBagSystem = InventorySystemStorage.Instance.GetSystem(InventoryType.Loot);
        lootBagSystem.OnInventoryDataModified += LootBagDisplayInventory_OnInventoryDataChange;
    }


    // Show Loot
    public void Interact()
    {
        // Debug.Log("Interacted with Loot Bag: " + gameObject.transform.parent.name);

        if (lootBagSystem == null)
        {
            Debug.Log("Inventory System to display loot in loot bag is null.");
            return;
        }

        // Set the UI of the inventory system to be active.
        if (!lootBagSystem.isActiveAndEnabled)
        {
            lootBagSystem.gameObject.SetActive(true);
        }

        // Set the inventory data of the loot bag system to the loot bag's inventory data.
        lootBagSystem.SetInventoryData(lootBagDisplayInventory);

    }

    public void UnInteract()
    {
        if (lootBagSystem != null)
        {
            lootBagSystem.gameObject.SetActive(false);
        }
    }

    // TODO: handle this logic somewhere else. Also considering using a pool system to prevent
    // excessive instantiation and destruction of loot bags.
    // Check if inventory is empty. If so, destroy the loot bag.
    private void LootBagDisplayInventory_OnInventoryDataChange()
    {
        
        if (lootBagDisplayInventory.IsEmpty())
        {
            if (parentObject.activeSelf == false)
            {
                return;
            }

            StartCoroutine(DeactivateLootbag());

            // This works for preventing bug. 
            // transform.parent.gameObject.transform.position = new Vector3(0, -100, 0);
        }
    }

    // Known bug this coroutine fixes.
    // BUG: bug where when player is touching more than 1 loot bag, emptying out one, destroys the other 
    // as well. Could be something to do with the SO being empty during the process of emptying out the loot bag?
    // And since the event gets called twice, it destroys the other loot bag as well.
    private IEnumerator DeactivateLootbag()
    {
        yield return new WaitForSeconds(0.2f);
        if (lootBagDisplayInventory.IsEmpty())
        {
            if (parentObject.activeSelf != false)
            {
                Debug.Log("Loot bag empty. Destroy loot bag! " + gameObject.transform.parent.name);
                parentObject.SetActive(false);
            }
        }
    }

}
