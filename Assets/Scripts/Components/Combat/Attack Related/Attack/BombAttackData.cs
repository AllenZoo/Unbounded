using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "new Bomb Attack Data", menuName = "System/Combat/Attack/BombAttack", order = 1)]
public class BombAttackData : AttackData
{
    [Tooltip("Radius of explosion effect")]
    public float explosionRadius;

    [Tooltip("Time in seconds before bomb explodes")]
    public float fuseTime;
}
