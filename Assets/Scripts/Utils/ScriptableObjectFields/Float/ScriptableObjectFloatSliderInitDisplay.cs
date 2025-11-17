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

    private void Start()
    {
        slider.value = floatObj.Value;

        floatObj.OnValueChanged += (float val) => slider.value = val; 
    }
}
