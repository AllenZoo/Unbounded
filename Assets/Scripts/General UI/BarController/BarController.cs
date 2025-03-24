using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    [Tooltip("The local event handler belonging to the entity whose stats we are tracking")]
    [SerializeField] public LocalEventHandler localEventHandler;
    [SerializeField] private StatComponent statObject;

    [SerializeField] private BarTrackStat statToTrack;
    [SerializeField] private Image fillImage;

    private void Awake()
    {
        Assert.IsNotNull(fillImage, "Bar controller needs a fill image");
        // Assert.IsNotNull(statObject, "Bar controller needs a stat object");
    }

    private void Start()
    {
        if (statObject != null)
        {
            Set(localEventHandler, statObject, statToTrack);
        }
        Render();
    }

    public void Set(LocalEventHandler eventHandler, StatComponent statObject, BarTrackStat statToTrack)
    {
        this.statObject = statObject;
        this.statToTrack = statToTrack;
        this.localEventHandler = eventHandler;

        LocalEventBinding<OnStatChangeEvent> statModResBinding = new LocalEventBinding<OnStatChangeEvent>(OnStatChange);
        localEventHandler.Register(statModResBinding);

        Render();
    }
    private void OnStatChange(OnStatChangeEvent e)
    {
        // Update the bar to reflect the new stat
        statObject = e.statComponent;
        Render();
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