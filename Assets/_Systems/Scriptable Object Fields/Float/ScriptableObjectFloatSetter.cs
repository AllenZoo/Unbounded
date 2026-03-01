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
    [SerializeField] private float multiplier = 1f;
    public void SetSOFloat(float value)
    {
        var valToSet = Mathf.RoundToInt(value * multiplier);

        if (valToSet != obj.Value)
        {
            obj.Set(valToSet);
        }
    }
}
