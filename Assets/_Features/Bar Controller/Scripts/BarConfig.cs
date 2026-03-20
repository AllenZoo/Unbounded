using UnityEngine;

/// <summary>
/// DTO class that contains information necessary to BarView. (from BarController)
/// </summary>
public class BarConfig
{
    public float CurrentValue { get; private set; }
    public float MaxValue { get; private set; }
    public string BarValueText { get; private set; } // set to "" if none
    public string BarOwnerText { get; private set; } // set to "" if none
    public BarConfig(float currentValue, float maxValue, string displayText, string barOwnerText)
    {
        CurrentValue = currentValue;
        MaxValue = maxValue;
        BarValueText = displayText;
        BarOwnerText = barOwnerText;
    }
}
