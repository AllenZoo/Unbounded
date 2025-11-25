using UnityEngine;

public class BombAttack : IAttack
{
    public AttackData AttackData { get => bombAttackData; set => bombAttackData = (BombAttackData) value; }

    private BombAttackData bombAttackData;

    public BombAttack() { }

    public void Hit(Damageable hit, Transform hitMaker)
    {
        throw new System.NotImplementedException();
    }

    public void OnLaunch()
    {
        throw new System.NotImplementedException();
    }

    public void SetModifiers(float atkStat, double percentageDamageIncrease)
    {
        throw new System.NotImplementedException();
    }
}
