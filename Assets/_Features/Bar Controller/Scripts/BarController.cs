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
public class BarController : MonoBehaviour
{
    [Tooltip("The local event handler belonging to the entity whose stats we are tracking. Could be overriden (eg. bosses) but initial connection here!")]
    [SerializeField, Required] public LocalEventHandlerContext localEventHandlerContext;
    [SerializeField, Required] public BarContext barContext;
    [Tooltip("False for player, true for bosses")]
    [SerializeField] protected bool useBarContext; // false for player, true for bosses

    [SerializeField, Required] public GameObject displayUI;
    [SerializeField, Required] protected BarTrackStat statToTrack;
    [SerializeField, Required, ValidateInput(nameof(ValidateIsChild), "fillImage must be a child of displayUI!")]
    protected Image fillImage;

    protected StatComponent statObject;
    protected LocalEventHandler leh;
    protected LocalEventBinding<OnStatChangeEvent> statModResBinding;

    protected void Awake()
    {
        Assert.IsNotNull(localEventHandlerContext, "Bar Controller needs a local event handler context!");
        Assert.IsNotNull(barContext, "Bar Controller needs a bar context!");
        Assert.IsNotNull(fillImage, "Bar controller needs a fill image");
    }

    protected void Start()
    {
        //localEventHandlerContext.OnInitialized += OnLEHInit;
        //OnLEHInit(); // this call is in case we don't subscribe before OnInitialized gets called in LEH.
        //if (useBarContext) { 
        //    barContext.OnBarContextChange += Render;
        //    barContext.OnBarContextChange += OnBarContextChange;
        //}
        //Render();
    }

    protected void OnEnable()
    {
        localEventHandlerContext.OnInitialized += OnLEHInit;

        if (useBarContext)
        {
            barContext.OnBarContextChange += Render;
            barContext.OnBarContextChange += OnBarContextChange;
        }

        OnLEHInit();
        Render();
    }

    protected void OnDisable()
    {
        localEventHandlerContext.OnInitialized -= OnLEHInit;

        if (useBarContext)
        {
            barContext.OnBarContextChange -= Render;
            barContext.OnBarContextChange -= OnBarContextChange;
        }

        ClearSubscriptions();
    }


    #region Helpers

    /// <summary>
    /// Main setter. Clears previous subscriptions from previous LEH and 
    /// handles subscriptions for new one :)
    /// </summary>
    /// <param name="leh"></param>
    public void SetLEH(LocalEventHandler leh)
    {
        ClearSubscriptions();
        this.leh = leh;
        Register();
        Render();
    }

    protected void OnStatChange(OnStatChangeEvent e)
    {
        // Update the bar to reflect the new stat
        statObject = e.statComponent;
        Render();
    }

    protected void OnBarContextChange()
    {
        if (leh != null && leh.Equals(barContext.LEH)) return;           
        SetLEH(barContext.LEH);
    }

    /// <summary>
    /// Helper function to bind the new LEH.
    /// </summary>
    protected void OnLEHInit()
    {
        if (localEventHandlerContext.Initialized && localEventHandlerContext.LocalEventHandler != null)
        {
            SetLEH(localEventHandlerContext.LocalEventHandler);
        }
    }

    protected void Register()
    {
        if (leh != null)
        {
            // Subscribe Stat Change to OnStatChange()
            statModResBinding = new LocalEventBinding<OnStatChangeEvent>(OnStatChange);
            leh.Register(statModResBinding);
        }
    }

    /// <summary>
    /// Helper to unregister any subscriptions relevant to curent local event handler.
    /// </summary>
    protected void ClearSubscriptions()
    {
        if (leh != null && statModResBinding != null)
        {
            leh.Unregister(statModResBinding);
            statModResBinding = null;
        }

        leh = null;
    }


    protected virtual void Render()
    {
        if (!this) return;
        if (!isActiveAndEnabled) return;
        if (!fillImage) return;

        if (useBarContext)
        {
            displayUI?.SetActive(barContext.IsVisible);
        }

        if (statObject == null || statObject.StatContainer == null) return;

        float fill = 0f;

        switch (statToTrack)
        {
            case BarTrackStat.HP:
                float max = statObject.StatContainer.MaxHealth;
                fill = max > 0 ? statObject.StatContainer.Health / max : 0f;
                break;
        }
        fillImage.fillAmount = Mathf.Clamp01(fill);
    }
    #endregion

    #region Validators

    protected bool ValidateIsChild(GameObject obj)
    {
        if (obj == null || displayUI == null) return false; // Ensures both fields are assigned
        return obj.transform.IsChildOf(displayUI.transform); // Checks if displayImage is a child of displayUI
    }

    #endregion
}

public enum BarTrackStat
{
    HP,
    MP,
    Stamina
}