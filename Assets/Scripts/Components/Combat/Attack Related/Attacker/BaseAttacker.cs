using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Attacker class shared by all attackers.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseAttacker<T>: IAttacker, IAttackNode where T: AttackerData
{
    [Required, OdinSerialize] protected T attackerData;
    public AttackerData AttackerData { get => attackerData; set => attackerData = (T)value; }
    public abstract AttackData AttackData { get; set; }

    /// <summary>
    /// Current multiplier math: Every 10 dex = 0.25f reduction in cooldown. For reference, fireball staff attack has 0.5s cooldown, meaning that 10 dex would reduce the cooldown by half.
    /// This is subject to change as we playtest and balance the game, but for now this is the formula we are using.
    /// </summary>
    private const float DEX_MULTIPLIER = 0.025f;

    public abstract void Attack(KeyCode keyCode, AttackContext attackContext);
    public abstract void StopAttack();
    public abstract IAttacker DeepClone();
    public abstract float GetChargeUp();
    public abstract IEnumerable<IAttackNode> GetChildren();
    public virtual float GetCooldown(float dexterity) {
        return GetCooldownLinear(attackerData.cooldown, dexterity, DEX_MULTIPLIER);
    }
    public abstract bool IsInitialized();

    protected float GetCooldownLinear(float baseCooldown, float dex, float dexMultiplier = 0.01f)
    {
        return Mathf.Max(0.05f, baseCooldown - (dex * dexMultiplier));
    }
}
