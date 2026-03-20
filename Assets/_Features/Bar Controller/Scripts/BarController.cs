using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class BarController : IController<BarChannel, BarView, BarConfig>
{
    protected BarView view;
    protected BarChannel model;
    protected bool displayValueText; 

    protected BarController(BarView view, BarChannel model, bool displayValueText)
    {
        this.view = view;
        this.model = model;
        this.displayValueText = displayValueText;
    }

    public class Builder
    {
        private BarChannel model = null;   
        private bool displayValueText = false;

        public Builder WithModel(BarChannel model) { 
            this.model = model;
            return this;
        }

        public Builder WithConfig(bool displayValueText)
        {
            this.displayValueText = displayValueText;
            return this;
        }
       
        public BarController Build(BarView view)
        {
            Assert.IsNotNull(view, "View cannot be null when building BarController!");
            return new BarController(view, model, displayValueText);
        }
    }
    
    public void Cleanup() { }

    public void UpdateState(BarChannel model)
    {
        this.model = model;
        UpdateView();
    }

    public void UpdateView()
    {
        if (model == null || view == null) return;

        var viewConfig = CreateConfig(model);
        view.UpdateView(viewConfig);

        if (model.Stat != null && model.IsVisible)
        {
            view.ShowView();
        }
        else
        {
            view.HideView();
        }
    }

    public BarConfig CreateConfig(BarChannel model)
    {
        if (model == null || model.Stat == null || model.Stat.StatContainer == null)
        {
            return new BarConfig(0, 1, "", "");
        }

        var statContainer = model.Stat.StatContainer;
        var valueText = displayValueText ? $"{Mathf.CeilToInt(statContainer.Health)}/{Mathf.CeilToInt(statContainer.MaxHealth)}" : "";
        
        // Display bar owner text only for bosses
        var barOwnerText = (model.BossBarConfig != null) ? model.BossBarConfig.BossName : "";

        return new BarConfig(statContainer.Health, statContainer.MaxHealth, valueText, barOwnerText);
    }
}

public enum BarTrackStat
{
    HP,
    MP,
    Stamina
}