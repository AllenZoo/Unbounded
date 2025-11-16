using TMPro;
using UnityEngine;

/// <summary>
/// Maps a SerializableObject (scriptable object) to text.
/// </summary>
public class ScriptableObjectFloatTextDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private ScriptableObjectFloat val;

    private void Start()
    {
        text.text = val.Value.ToString();
        val.OnValueChanged += (float val) => text.text = val.ToString();
    }
}
