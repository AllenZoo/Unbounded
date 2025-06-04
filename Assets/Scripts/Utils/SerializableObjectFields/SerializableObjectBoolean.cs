using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boolean Variable", menuName = "System/Serializable Object Variables/Boolean")]
public class SerializableObjectBoolean : ScriptableObject
{
    [SerializeField] private bool value;
    public bool Value => value;

    public event Action OnValueChanged;

    public void Set(bool newValue)
    {
        if (value == newValue) return;

        value = newValue;
        OnValueChanged?.Invoke();
    }
}
