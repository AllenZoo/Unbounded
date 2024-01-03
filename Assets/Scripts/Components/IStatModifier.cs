using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IStatModifier
{
    [SerializeField] private float value;
    [SerializeField] private Stat stat;

    public float Value { get { return value; } }
    public Stat Stat { get { return stat; } }

    public IStatModifier(Stat stat, float value)
    {
        this.value = value;
        this.stat = stat;
    }
}
