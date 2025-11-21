using System.Collections.Generic;
using UnityEngine;

public interface IAttacker
{
    public void Attack(KeyCode keyCode, AttackSpawnInfo info, Transform attackerTransform, List<EntityType> targetTypes, float atkStat, double percentageDamageIncrease);
    public bool IsInitialized();
    public float GetCooldown();
    public float GetChargeUp();
    //public AttackerData AttackerData { get; set; }
    //public AttackData AttackData { get; set; }
}
