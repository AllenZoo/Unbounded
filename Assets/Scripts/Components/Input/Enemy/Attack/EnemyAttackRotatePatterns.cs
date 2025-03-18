using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new EnemyAttack Rotate Patterns", menuName = "System/Enemy/State/Attack/AttackRotatePatterns")]
public class EnemyAttackRotatePatterns : EnemyAttackSOBase
{
    // TODO: list tuple of attacker and expected EnemyChaseSOBase behaviour.

    [SerializeField]
    private List<Attacker> attackers = new List<Attacker>();


    // TODO: to equip send out onAttackPatternChangeEvent(Attacker newAttackPattern)
    
}
