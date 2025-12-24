using UnityEngine;

public interface IAttack {
    // Should be called when the attack is launched/spawned
    void OnLaunch(AttackComponent ac);

    // Should be called when the attack lands/hits the ground
    void OnLand(AttackComponent ac);

    // Should be called when the attack hits a target
    bool Hit(Damageable hit, Transform hitMaker);

    void SetModifiers(float atkStat, double percentageDamageIncrease);

    void Reset(AttackComponent ac);

    public AttackData AttackData { get; set; }

     
}
