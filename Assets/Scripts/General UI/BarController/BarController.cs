using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private bool useBarContext; // false for player, true for bosses

    [SerializeField, Required] public GameObject displayUI;
    [SerializeField, Required] private BarTrackStat statToTrack;
    [SerializeField, Required, ValidateInput(nameof(ValidateIsChild), "fillImage must be a child of displayUI!")] 
    private Image fillImage;

    private StatComponent statObject;
    private LocalEventHandler leh;
    private LocalEventBinding<OnStatChangeEvent> statModResBinding;

    private void Awake()
    {
        Assert.IsNotNull(localEventHandlerContext, "Bar Controller needs a local event handler context!");
        Assert.IsNotNull(barContext, "Bar Controller needs a bar context!");
        Assert.IsNotNull(fillImage, "Bar controller needs a fill image");
    }

    private void Start()
    {
        localEventHandlerContext.OnInitialized += OnLEHInit;
        OnLEHInit(); // this call is in case we don't subscribe before OnInitialized gets called in LEH.
        if (useBarContext) { 
            barContext.OnBarContextChange += Render;
            barContext.OnBarContextChange += OnBarContextChange;
        }
        Render();
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
        OnLEHInit();
        Render();
    }

    private void OnStatChange(OnStatChangeEvent e)
    {
        // Update the bar to reflect the new stat
        statObject = e.statComponent;
        Render();
    }

    private void OnBarContextChange()
    {
        if (leh != null && leh.Equals(barContext.LEH)) return;           
        SetLEH(barContext.LEH);
    }

    /// <summary>
    /// Helper function to bind the new LEH.
    /// </summary>
    private void OnLEHInit()
    {
        // Redundant check but just to make sure.
        // Update: not redundant anymore hehe.
        // Update2: added extra check to see that lehc.leh is not null since the way we call this function, it could be.
        if (localEventHandlerContext.Initialized && localEventHandlerContext.LocalEventHandler != null)
        {

            // Subscribe Stat Change to OnStatChange()
            LocalEventHandler handler = localEventHandlerContext.LocalEventHandler;
            statModResBinding = new LocalEventBinding<OnStatChangeEvent>(OnStatChange);
            handler.Register(statModResBinding);
        }
    }

    /// <summary>
    /// Helper to unregister any subscriptions relevant to curent local event handler.
    /// </summary>
    private void ClearSubscriptions()
    {
        localEventHandlerContext.OnInitialized -= OnLEHInit;
        LocalEventHandler handler = localEventHandlerContext.LocalEventHandler;
        handler.Unregister(statModResBinding);
    }

    private void Render()
    {
        if (useBarContext)
        {
            displayUI.SetActive(barContext.IsVisible);
        }

        if (statObject == null) return;

        switch (statToTrack)
        {
            case BarTrackStat.HP:
                fillImage.fillAmount = statObject.health / statObject.maxHealth;
                break;
            case BarTrackStat.MP:
                fillImage.fillAmount = statObject.mana / statObject.maxMana;
                break;
            case BarTrackStat.Stamina:
                fillImage.fillAmount = statObject.stamina / statObject.maxStamina;
                break;
        }
        
    }
    #endregion

    #region Validators

    private bool ValidateIsChild(GameObject obj)
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