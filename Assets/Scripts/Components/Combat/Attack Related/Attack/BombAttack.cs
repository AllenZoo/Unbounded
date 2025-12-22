using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class BombAttack : IAttack
{
    public AttackData AttackData { get => bombAttackData; set => bombAttackData = (BombAttackData) value; }

    [Required, OdinSerialize] private BombAttackData bombAttackData;

    public BombAttack() { }

    public void Hit(Damageable hit, Transform hitMaker)
    {
        // if not landed yet, dont do anything.
        throw new System.NotImplementedException();
    }

    public void OnLaunch()
    {
        throw new System.NotImplementedException();
    }

    public void OnLand(MonoBehaviour corountineStarter)
    {
        // Start fuse timer.
        //bombAttackData.fuseTime;
        throw new System.NotImplementedException();
    }

    public IEnumerator OnLandEnumerator()
    {
        yield return new WaitForSeconds(bombAttackData.fuseTime);
        Explode();
    }

    public void Explode()
    {
        // explode and do damage to any entity in radius. (e.g. collider)
        // Do this by spawning an explosion attack overtop.

    }

    public void SetModifiers(float atkStat, double percentageDamageIncrease)
    {
        throw new System.NotImplementedException();
    }
}
