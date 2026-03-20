using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

/// <summary>
/// Bar Controller class that handles displaying the UI.
/// 
/// Currently Player uses System1:
///     - initialize once through localEventHandlerContext and don't change.
///    
/// Bosses use System 2:
///     - initialize in beginning similar to player via lehContext
///     - reinitialize on AGGRO via BossBattleHealthBarSpawner.
/// </summary>
public class BarController : IController<BarChannel, BarView, BarConfig>
{
    protected BarView view;
    protected BarChannel model;
    protected bool displayValueText; // essentially a config. Eventually make into class when we have more config options.

    protected BarController(BarView view, BarChannel model, bool displayValueText)
    {
        this.view = view;
        this.model = model;
        this.displayValueText = displayValueText;

        ConnectView();
        ConnectModel();
    }

    public class Builder
    {
        private BarChannel model = new();   
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
    

    private void ConnectModel()
    {
        // Subscribe to any relevant model events here.
        
        // TODO; when structure is clear, maybe we would subscribe to some model.OnChange event, but currently we just depedn on LEH sub.
    }

    private void ConnectView()
    {
        // Subscribe to any relevant view events here.
    }

    public void Cleanup()
    {
        // Unsubscribe from all events here.
    }

    public void UpdateState(BarChannel model)
    {
        this.model = model;
        UpdateView();
    }

    public void UpdateView()
    {
        var viewConfig = CreateConfig(model);
        view.UpdateView(viewConfig);
        view.ShowView();
    }

    public BarConfig CreateConfig(BarChannel model)
    {
        // Default to tracking HP. Can add more stats later if needed.
        var statContainer = model.Stat.StatContainer;

        var valueText = $"{statContainer.Health}/{statContainer.MaxHealth}";
        if (!displayValueText)
        {
            valueText = "";
        }

        // Display bar owner text only for bosses, for player we can just leave it blank or display player name if needed.
        var barOwnerText = model.BossBarConfig != null ? model.BossBarConfig.name : "";


        BarConfig config = new BarConfig(statContainer.Health, statContainer.MaxHealth, valueText, barOwnerText);

        return config;
    }
}

public enum BarTrackStat
{
    HP,
    MP,
    Stamina
}