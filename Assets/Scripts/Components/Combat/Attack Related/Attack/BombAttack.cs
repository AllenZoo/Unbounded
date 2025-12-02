using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

[Serializable]
public class BombAttack : IAttack
{
    public AttackData AttackData { get => bombAttackData; set => bombAttackData = (BombAttackData) value; }

    [Required, OdinSerialize] private BombAttackData bombAttackData;

    public BombAttack() { }

    public void Hit(Damageable hit, Transform hitMaker)
    {
        throw new System.NotImplementedException();
    }

    public void OnLaunch()
    {
        throw new System.NotImplementedException();
    }

    public void OnLand()
    {
        throw new System.NotImplementedException();
    }

    public void SetModifiers(float atkStat, double percentageDamageIncrease)
    {
        throw new System.NotImplementedException();
    }
}
