using UnityEngine;

public class BombAttack : IAttack
{
    public BombAttackData BombAttackData { get => bombAttackData; set => bombAttackData = value; }
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
}
