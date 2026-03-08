using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Glue component that links up controller with view and model.
/// 
/// May also connect to InventorySystem if we want to initialize with InventorySystem's model.
/// 
/// </summary>
public class EquipmentSystem : MonoBehaviour //InventorySystem
{
    [Required, SerializeField] private EquipmentView view;

    [SerializeField] private bool initializeWithInventorySystem = false;
    [Required, SerializeField, ShowIf("initializeWithInventorySystem")] private InventorySystem inventorySystem;

    [SerializeField] private bool initializeWithInventoryData = false;
    [Required, SerializeField, ShowIf("initializeWithInventoryData")] private InventoryData inventoryData;

    private EquipmentController controller;

    // TODO: hook up to necessary eventbus events.

    private void Awake()
    {
        Assert.IsNotNull(view, "EquipmentView reference is not set in the inspector.");
        if (inventoryData != null && initializeWithInventoryData)
        {
            controller = new EquipmentController.Builder()
                .WithInitialInventory(inventoryData)
                .Build(view);
        } else if (inventorySystem != null  && initializeWithInventorySystem)
        {
            controller = new EquipmentController.Builder()
                .WithInventorySystem(inventorySystem)
                .Build(view);
        }
        else
        {
            controller = new EquipmentController.Builder()
                .WithoutInitialInventory()
                .Build(view);
        }
    }
}
