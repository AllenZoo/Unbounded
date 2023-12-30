using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IStatModifier
{
    public float Value { get { return value; } }
    public Stat Stat { get { return stat; } }
    private float value;
    private Stat stat;

    public IStatModifier(Stat stat, float value)
    {
        this.value = value;
        this.stat = stat;
    }
}
