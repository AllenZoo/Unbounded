using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class BarController : MonoBehaviour
{
    [SerializeField] private StatComponent statObject;
    [SerializeField] private Stat statToTrack;
    [SerializeField] private Image fillImage;

    private void Awake()
    {
        Assert.IsNotNull(fillImage, "Bar controller needs a fill image");
        // Assert.IsNotNull(statObject, "Bar controller needs a stat object");
    }

    private void Start()
    {
        statObject.OnStatChange += OnStatChange;
        Render();
    }

    public void Set(StatComponent statObject, Stat statToTrack)
    {
        this.statObject = statObject;
        this.statToTrack = statToTrack;
        statObject.OnStatChange += OnStatChange;
        Render();
    }
    private void OnStatChange(StatComponent statComponent, IStatModifier statModifier)
    {
        // Update the bar to reflect the new stat
        Render();
    }

    private void Render()
    {
        fillImage.fillAmount = statObject.GetCurStat(statToTrack) / statObject.GetMaxStat(statToTrack);
    }


}
