using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that initializes slider intial value.
/// Used in audio system
/// </summary>
public class ScriptableObjectFloatSliderInitDisplay : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private ScriptableObjectFloat floatObj;

    private void Start()
    {
        slider.value = floatObj.Value;
    }
}
