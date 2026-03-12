using UnityEngine;

public interface IAttack {
    // Should be called when the attack is launched/spawned
    void OnLaunch(AttackComponent ac);

    // Should be called when the attack lands/hits the ground (if applicable)
    void OnLand(AttackComponent ac);

    // Should be called when the attack hits a target
    /// <summary>
    /// Applies an attack to the specified target and determines whether the hit was successful.
    /// </summary>
    /// <param name="hit">The target to receive damage from the attack. Cannot be null.</param>
    /// <param name="attackPos">The position at which the attack object is. Used to determine hit location and effects. Cannot be null.</param>
    /// <param name="ac">The attack component providing details about the attack, such as damage type and modifiers. Cannot be null.</param>
    /// <param name="attackSource">The transform representing the source of the attack, typically the attacker. Cannot be null.</param>
    /// <returns>true if the attack successfully hits and applies its effects to the target; otherwise, false.</returns>
    bool Hit(Damageable hit, Transform attackPos, AttackComponent ac, Transform attackSource);

    // TODO: see if we can completely remove this function.
    void SetModifiers(float atkStat, double percentageDamageIncrease);

    void Reset(AttackComponent ac);

    public AttackData AttackData { get; set; }
}
