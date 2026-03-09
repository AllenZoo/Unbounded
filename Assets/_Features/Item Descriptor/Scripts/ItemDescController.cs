using NUnit.Framework;
using UnityEngine;

public class ItemDescController
{
    private ItemDescView view;
    private Item model;
   
    public ItemDescController(ItemDescView view, Item model)
    {
        this.view = view;
        this.model = model;

        ConnectModel();
        ConnectView();
    }

    public class Builder
    {
        private Item model = new Item();

        public Builder WithoutModel()
        {
            return this;
        }

        public Builder WithItemModel(Item model)
        {
            this.model = model;
            return this;
        }

        public ItemDescController Build(ItemDescView view)
        {
            Assert.IsNotNull(view);
            return new ItemDescController(view, model);
        }
    }

    private void ConnectModel()
    {
        // Subscribe to any relevant model events here.
    }

    private void ConnectView()
    {
        // Subscribe to any relevant view events here.
    }

    public void Cleanup()
    {
        // Unsubscribe from all events here.

    }

    public void UpdateModel(Item model)
    {
        this.model = model;
        UpdateView();
    }

    public void UpdateView()
    {
        ItemDescViewConfig viewConfig = GenerateItemDescViewConfig(model);
        view.DisplayView(viewConfig);
        ShowView();

    }

    /// <summary>
    /// Injects the model data and shows the descriptor.
    /// </summary>
    public void ShowView()
    {
        view.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the descriptor view.
    /// </summary>
    public void HideView()
    {
        view.gameObject.SetActive(false);
        model = null;
    }

    private ItemDescViewConfig GenerateItemDescViewConfig(Item model)
    {
        if (model.IsEmpty()) return null;

        var config = ItemDataConverter.ConvertFromItem(model);
        return config;
    }
}
