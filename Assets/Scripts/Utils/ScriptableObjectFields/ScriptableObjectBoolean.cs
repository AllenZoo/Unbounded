using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boolean Variable", menuName = "System/Scriptable Object Variables/Boolean")]
public partial class ScriptableObjectBoolean : ScriptableObject
{
    [SerializeField] private bool value;

    [Tooltip("Reset to default OnDisable")]
    [SerializeField] private bool resetToDefault = true;

    [Tooltip("The default value to reset to when needed.")]
    [SerializeField] private bool defaultValue;

    public bool Value => value;

    public event Action OnValueChanged;

    public void Set(bool newValue)
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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
partial class ScriptableObjectBoolean
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ResetAllBooleans()
    {
        var all = Resources.FindObjectsOfTypeAll<ScriptableObjectBoolean>();

        foreach (var boolean in all)
        {
            boolean.ResetValue();
        }
    }
}
#endif
