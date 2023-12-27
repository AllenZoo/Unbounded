using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// Modifies state of UI based on inventory data. Also handles user input.
public class InventoryUI : MonoBehaviour
{
    [SerializeField] private SO_Inventory inventoryData;
    [SerializeField] private GameObject inventorySlotParent;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject inventoryTitle;


    private void Awake()
    {
        Assert.IsNotNull(inventoryData, "Need inventory data for UI to reflect the its state.");
    }

    // TODO: On inventory data change, rerender UI.
    public void Rerender()
    {

    }
}
