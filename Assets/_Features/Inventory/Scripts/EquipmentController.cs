using NUnit.Framework;
using Unity.InferenceEngine;
using UnityEngine;

/// <summary>
/// The main controller class of MVC model for displaying equipment.
/// </summary>
public class EquipmentController
{
    private Inventory model;
    private EquipmentView view;

    public EquipmentController(Inventory model, EquipmentView view)
    {
        this.view = view;
        this.model = model;

        ConnectModel();
        ConnectView();
    }

    public class Builder
    {
        private Inventory model = new Inventory();

        public Builder WithInitialInventory(InventoryData data)
        {
            model.Init(data);
            return this;
        }

        public Builder WithoutInitialInventory()
        {
            return this;
        }

        public EquipmentController Build(EquipmentView view)
        {
            Assert.IsNotNull(view);
            return new EquipmentController(model, view);
        }
    }

    public void Cleanup()
    {
        // Unsubscribe from any Model or View Events here.
    }

    private void ConnectModel()
    {
        // Subscribe to any Model Events here.
    }

    private void ConnectView()
    {
        // Subscribe to any View Events here.
    }
    private EquipmentViewConfig GenerateEquipmentViewConfig(Inventory inventory)
    {
        string weaponName = "";
        string weaponDescription = "";
        if (inventory.Items[0] != null)
        {
            weaponName = inventory.Items[0].Data.itemName;
            weaponDescription = inventory.Items[0].Data.description;
        }
        return new EquipmentViewConfig("<u>Equipment</u>", weaponName, weaponDescription);
    }
}
