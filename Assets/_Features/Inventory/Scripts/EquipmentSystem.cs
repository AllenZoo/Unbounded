using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Glue component that links up controller with view and model.
/// 
/// May also connect to InventorySystem if we want to initialize with InventorySystem's model.
/// 
/// This script also acts as the main entrypoint via EventBus if needed in future implementations.
/// </summary>
public class EquipmentSystem : MonoBehaviour
{
    // This PageUI reference is used to control the visibility and sorting order of the equipment UI page within the UI overlay system.
    [Required, SerializeField] private EquipmentView view;

    [SerializeField] private EquipmentInitializationMode initMode;
    private enum EquipmentInitializationMode
    {
        None,
        InventorySystem,
        InventoryData
    }

    [ShowIf(nameof(initMode), EquipmentInitializationMode.InventorySystem)]
    [Required, SerializeField] private InventorySystem inventorySystem;

    [ShowIf(nameof(initMode), EquipmentInitializationMode.InventoryData)]
    [Required, SerializeField] private InventoryData inventoryData;

    private EquipmentController controller;

    private void Awake()
    {
        Assert.IsNotNull(view, "EquipmentView reference is not set in the inspector.");
        if (inventoryData != null && initMode.Equals(EquipmentInitializationMode.InventoryData))
        {
            // Initialize with InventoryData.
            controller = new EquipmentController.Builder()
                .WithInitialInventory(inventoryData)
                .Build(view);
        } else if (inventorySystem != null && initMode.Equals(EquipmentInitializationMode.InventorySystem))
        {
            // Initialize with InventorySystem.
            // If we're initializing with the InventorySystem, we need to make sure it's initialized before we build the controller.
            inventorySystem.Init();
            controller = new EquipmentController.Builder()
                .WithInventorySystem(inventorySystem)
                .Build(view);
        }
        else
        {
            // Initialize with empty inventory.
            controller = new EquipmentController.Builder()
                .WithoutInitialInventory()
                .Build(view);
        }
    }
}

