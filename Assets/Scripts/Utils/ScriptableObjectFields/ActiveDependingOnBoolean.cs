using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ActiveDependingOnBoolean : MonoBehaviour
{
    [SerializeField, Required] private ScriptableObjectBoolean boolean;
    [SerializeField, Required] private GameObject objectToSet;

    private void Awake()
    {
        Assert.IsNotNull(boolean);
        Assert.IsNotNull(objectToSet);
    }

    private void Start()
    {
        boolean.OnValueChanged += Boolean_OnValueChanged;
        Boolean_OnValueChanged();
    }

    private void OnDestroy()
    {
        boolean.OnValueChanged -= Boolean_OnValueChanged;
    }

    private void OnDisable()
    {
        boolean.OnValueChanged -= Boolean_OnValueChanged;
    }

    private void Boolean_OnValueChanged()
    {
        objectToSet.SetActive(boolean.Value);
    }
}
