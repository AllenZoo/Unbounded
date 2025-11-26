using System;
using UnityEngine;

[Serializable]
public class CircleAttackIndicator : IAttackIndicator
{
    public CircleAttackIndicatorData Data { get { return data; } set { data = value; } }
    private CircleAttackIndicatorData data;

    public CircleAttackIndicator() { }


    public void Indicate()
    {

    }
}
