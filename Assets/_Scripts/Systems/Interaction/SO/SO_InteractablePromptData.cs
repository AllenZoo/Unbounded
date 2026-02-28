using Sirenix.OdinInspector;
using System;
using UnityEngine;

/// <summary>
/// ScriptableObject that holds InteractablePromptData. Fires an event whenever the data is changed.
/// </summary>
[CreateAssetMenu(fileName = "NewInteractablePromptData", menuName = "System/Interaction/InteractablePromptData")]
public class SO_InteractablePromptData : ScriptableObject
{
    public event Action OnDataChanged = delegate { };


    [SerializeField, ReadOnly]
    private InteractablePromptData data;

    public InteractablePromptData Data
    {
        get => data;
        private set => data = value;
    }

    public void SetData(InteractablePromptData newData)
    {
        if (data.Equals(newData)) return; // Structs use value equality, so this prevents redundant updates

        Data = newData;
        OnDataChanged.Invoke();
    }
}
