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
public class BarController : IController<StatContainer, BarView, BarConfig>
{
    protected BarView view;
    protected StatContainer model;

    protected BarController(BarView view, StatContainer model)
    {
        this.view = view;
        this.model = model;

        ConnectView();
        ConnectModel();
    }

    public class Builder
    {
        private StatContainer model;    

        public Builder WithModel(StatContainer model) { 
            this.model = model;
            return this;
        }
       
        public BarController Build(BarView view)
        {
            Assert.IsNotNull(view, "View cannot be null when building BarController!");
            return new BarController(view, model);
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

    public void UpdateState(StatContainer model)
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

    public BarConfig CreateConfig(StatContainer model)
    {
        throw new System.NotImplementedException();
    }
}

public enum BarTrackStat
{
    HP,
    MP,
    Stamina
}