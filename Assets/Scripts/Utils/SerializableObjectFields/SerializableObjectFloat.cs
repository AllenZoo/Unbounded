using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Float Variable", menuName = "System/Serializable Object Variables/Float")]
public class SerializableObjectFloat : ScriptableObject
{
    [SerializeField] private float value;

    [Tooltip("Reset to default OnDisable")]
    [SerializeField] private bool resetToDefault = true;

    [Tooltip("The default value to reset to when needed.")]
    [SerializeField] private float defaultValue;

    public float Value => value;

    public event Action OnValueChanged;

    public void Set(float newValue)
    {
        if (value == newValue) return;

        value = newValue;
        OnValueChanged?.Invoke();
    }

    /// <summary>
    /// Resets the value to its default if enabled.
    /// </summary>
    public void ResetValue()
    {
        if (!resetToDefault) return;

        Set(defaultValue);
    }

    /// <summary>
    /// Optional helper to set the current value as the default (can be used in editor).
    /// </summary>
    public void SetCurrentAsDefault()
    {
        defaultValue = value;
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        ResetValue();
#endif
    }
}
