using UnityEngine;

/// <summary>
/// DTO class that contains information necessary to BarView. (from BarController)
/// </summary>
public class BarConfig
{
    public float CurrentValue { get; private set; }
    public float MaxValue { get; private set; }
    public string DisplayText { get; private set; }
    public BarConfig(float currentValue, float maxValue, string displayText)
    {
        CurrentValue = currentValue;
        MaxValue = maxValue;
        DisplayText = displayText;
    }
}
