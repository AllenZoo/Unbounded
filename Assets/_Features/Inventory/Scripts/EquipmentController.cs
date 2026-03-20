using UnityEngine.Assertions;
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

        UpdateView();
    }

    public class Builder
    {
        private Inventory model = new Inventory();

        public Builder WithInitialInventory(InventoryData data)
        {
            model.Init(data);
            return this;
        }

        public Builder WithInventorySystem(InventorySystem inventorySystem)
        {
            model = inventorySystem.Inventory;
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

    public void SetModel(Inventory newModel)
    {
        if (model != null)
        {
            model.OnInventoryDataModified -= HandleModelModified;
        }

        model = newModel;

        if (model != null)
        {
            model.OnInventoryDataModified += HandleModelModified;
        }

        UpdateView();
    }

    public void Cleanup()
    {
        // Unsubscribe from any Model or View Events here.
        model.OnInventoryDataModified -= HandleModelModified;
    }

    private void ConnectModel()
    {
        // Subscribe to any Model Events here.
        model.OnInventoryDataModified += HandleModelModified;
    }

    private void ConnectView()
    {
        // Subscribe to any View Events here.
    }

    private void UpdateView()
    {
        var config = GenerateEquipmentViewConfig(model);
        view.UpdateView(config);
    }

    private void HandleModelModified()
    {
        UpdateView();
    }

    private EquipmentViewConfig GenerateEquipmentViewConfig(Inventory inventory)
    {
        string weaponName = "No Weapon Equipped";
        string weaponDescription = "\"No Weapon? Guess I'll just have to run around...\"";
        if (!inventory.GetItem(0).IsEmpty())
        {
            weaponName = inventory.GetItem(0).Data.itemName;
            weaponDescription = inventory.GetItem(0).Data.description;
        }
        return new EquipmentViewConfig("<u>Equipment</u>", weaponName, weaponDescription);
    }
}
