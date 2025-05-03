using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct Optional<T> where T : class
{
    public T Value { get; }
    public bool HasValue => Value != null;

    public Optional(T value)
    {
        Value = value;
    }

    public static implicit operator Optional<T>(T value) => new Optional<T>(value);
}

