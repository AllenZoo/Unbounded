using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOperation
{
    float Calculate(float value);
    float GetValue();
}

[Serializable]
public class AddOperation : IOperation
{
    readonly float value;

    public AddOperation(float value)
    {
        this.value = value;
    }

    public float Calculate(float value)
    {
        return this.value + value;
    }

    public float GetValue()
    {
        return value;
    }
}

[Serializable]
public class MultiplyOperation : IOperation
{
    readonly float value;

    public MultiplyOperation(float value)
    {
        this.value = value;
    }

    public float Calculate(float value)
    {
        return this.value + value;
    }

    public float GetValue()
    {
        return value;
    }
}
