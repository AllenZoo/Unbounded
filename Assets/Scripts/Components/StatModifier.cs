using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatModifier

{
    [SerializeField] private float value;
    [SerializeField] private Stat stat;

    public float Value { get { return value; } }
    public Stat Stat { get { return stat; } }

    public StatModifier(Stat stat, float value)
    {
        this.value = value;
        this.stat = stat;
    }
}
