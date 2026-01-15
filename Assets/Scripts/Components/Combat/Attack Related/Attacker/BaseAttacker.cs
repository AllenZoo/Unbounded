using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Attacker class shared by all attackers.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseAttacker<T>: IAttacker, IAttackNode where T: AttackerData
{
    [OdinSerialize] protected T attackerData;
    public AttackerData AttackerData { get => attackerData; set => attackerData = (T)value; }
    public abstract AttackData AttackData { get; set; }

    public abstract void Attack(KeyCode keyCode, AttackContext attackContext);
    public abstract void StopAttack();
    public abstract IAttacker DeepClone();
    public abstract float GetChargeUp();
    public abstract IEnumerable<IAttackNode> GetChildren();
    public abstract float GetCooldown();
    public abstract bool IsInitialized();
}
