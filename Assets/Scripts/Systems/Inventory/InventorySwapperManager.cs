using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Singleton that holds info about the swapping mechanism between inventory systems.
public class InventorySwapperManager : MonoBehaviour
{
    // Singleton
    public static InventorySwapperManager Instance;
    public int selectedSlotIndex = -1;
    public InventorySystem selectedSlotInventorySystem = null;
    public PointerEventData.InputButton inputButton;

    private void Awake()
    {
        // Check only one instance of this class exists
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.transform.parent.gameObject);
        }
    }

    public void ResetSelection()
    {
        selectedSlotIndex = -1;
        selectedSlotInventorySystem = null;
    }
}
