using System;
using UnityEngine;

public interface IOperation
{
    float Calculate(float value);
    float GetValue();
}

[Serializable]
public class AddOperation : IOperation
{
    [SerializeField] private float value = 1f;

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
    [SerializeField] private float value = 2f;

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
