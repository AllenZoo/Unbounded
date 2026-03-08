using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Glue component that links up controller with view and model.
/// </summary>
public class EquipmentSystem : MonoBehaviour
{
    [Required, SerializeField] private EquipmentView view;
    [SerializeField] private bool initializeWithInventoryData = false;
    [SerializeField, ShowIf("initializeWithInventoryData")] private InventoryData inventoryData;

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
        }
        else
        {
            controller = new EquipmentController.Builder()
                .WithoutInitialInventory()
                .Build(view);
        }
    }
}
