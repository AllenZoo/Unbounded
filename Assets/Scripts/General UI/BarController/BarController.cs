using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    [Tooltip("The local event handler belonging to the entity whose stats we are tracking")]
    [SerializeField, Required] public LocalEventHandlerContext localEventHandlerContext;
    [SerializeField, Required] private BarTrackStat statToTrack;
    [SerializeField, Required] private Image fillImage;

    private StatComponent statObject;

    private void Awake()
    {
        Assert.IsNotNull(localEventHandlerContext, "Bar Controller needs a local event handler context!");
        Assert.IsNotNull(fillImage, "Bar controller needs a fill image");
    }

    private void Start()
    {
        localEventHandlerContext.OnInitialized += OnLEHInit;
        Render();
    }

    private void OnStatChange(OnStatChangeEvent e)
    {
        // Update the bar to reflect the new stat
        statObject = e.statComponent;
        Render();
    }

    private void OnLEHInit()
    {
        // Redundant check but just to make sure.
        if (localEventHandlerContext.Initialized)
        {
            // Subscribe Stat Change to OnStatChange()
            LocalEventHandler handler = localEventHandlerContext.LocalEventHandler;
            LocalEventBinding<OnStatChangeEvent> statModResBinding = new LocalEventBinding<OnStatChangeEvent>(OnStatChange);
            handler.Register(statModResBinding);
        }
    }

    private void Render()
    {
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
}

public enum BarTrackStat
{
    HP,
    MP,
    Stamina
}