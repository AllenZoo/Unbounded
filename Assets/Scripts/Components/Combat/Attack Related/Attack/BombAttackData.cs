using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "new Bomb Attack Data", menuName = "System/Combat/Attack/BombAttack", order = 1)]
public class BombAttackData : AttackData
{
    [Header("Bomb Specific Fields")]
    [Tooltip("If true, always use the explosion radius defined in this field (explosionRadius).")]
    public bool alwaysUseThisExplosionRadius;

    [Tooltip("Radius of explosion effect")]
    public float explosionRadius; // TODO: this may need to be a dynamic field.. Think about adding an option that toggles this as dynamic or static.

    [Tooltip("Time in seconds before bomb explodes (after it lands)")]
    public float fuseTime;

    [Required]
    [Tooltip("Initial before explosion sprite")]
    public Sprite initSprite;

    [Required]
    [Tooltip("Bomb explosion sprite")]
    public Sprite explosionSprite;
}
