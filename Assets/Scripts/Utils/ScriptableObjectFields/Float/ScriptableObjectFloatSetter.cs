using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used for setting SO float object.
/// (e.g. Slider for audio system)
/// </summary>
public class ScriptableObjectFloatSetter : MonoBehaviour
{
    [Required, SerializeField] private ScriptableObjectFloat obj;
    [SerializeField] private float multiplier = 100f;
    public void SetSOFloat(float value)
    {
        obj.Set(Mathf.RoundToInt(value * multiplier));
    }
}
