using UnityEngine;

public interface IAttack {
    // Should be called when the attack is launched/spawned
    void OnLaunch();

    // Should be called when the attack hits a target
    void Hit(Damageable hit, Transform hitMaker);
}
