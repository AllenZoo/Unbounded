using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class OperationContainer
{
    [SerializeReference, InlineEditor, HideReferenceObjectPicker]
    [ValueDropdown(nameof(GetOperationOptions))]
    public IOperation operation;

    private IEnumerable<object> GetOperationOptions()
    {
        return new IOperation[]
        {
            new AddOperation(5f),
            new MultiplyOperation(3f)
        };
    }
}
