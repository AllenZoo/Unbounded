using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that initializes slider intial value.
/// 
/// Update: Also subscribes slider to any float changes.
/// Used in audio system
/// </summary>
public class ScriptableObjectFloatSliderInitDisplay : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private ScriptableObjectFloat floatObj;

    private Action<float> onValueChanged;

    private void OnEnable()
    {
        slider.value = floatObj.Value;

        if (onValueChanged != null)
        {
            floatObj.OnValueChanged -= onValueChanged;
        }

        onValueChanged = val => slider.value = val;
        floatObj.OnValueChanged += onValueChanged;
    }

    private void OnDisable()
    {
        floatObj.OnValueChanged -= onValueChanged;
    }
}
