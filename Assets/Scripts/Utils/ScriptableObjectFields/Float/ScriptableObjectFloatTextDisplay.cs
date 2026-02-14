using TMPro;
using UnityEngine;

/// <summary>
/// Maps a SerializableObject (scriptable object) to text.
/// </summary>
public class ScriptableObjectFloatTextDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private ScriptableObjectFloat val;

    private System.Action<float> onValueChanged;

    private void OnEnable()
    {
        text.text = val.Value.ToString();
        onValueChanged = v => text.text = v.ToString();
        val.OnValueChanged += onValueChanged;
    }

    private void OnDisable()
    {
        val.OnValueChanged -= onValueChanged;
    }
}
