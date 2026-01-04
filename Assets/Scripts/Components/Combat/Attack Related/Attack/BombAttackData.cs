using Sirenix.OdinInspector;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[CreateAssetMenu(fileName = "new Bomb Attack Data", menuName = "System/Combat/Attack/BombAttack", order = 1)]
public class BombAttackData : AttackData
{
    #region Bomb Attack Fields
    [FoldoutGroup("Bomb Attack Properties")]
    [Tooltip("If true, always use the explosion radius defined in this field (explosionRadius).")]
    public bool AlwaysUseThisExplosionRadius;

    [FoldoutGroup("Bomb Attack Properties")]
    [Tooltip("Radius of explosion effect")]
    public float ExplosionRadius; // TODO: this may need to be a dynamic field.. Think about adding an option that toggles this as dynamic or static.

    [FoldoutGroup("Bomb Attack Properties")]
    [Tooltip("Time in seconds before bomb explodes (after it lands)")]
    public float FuseTime;

    [FoldoutGroup("Bomb Attack Properties")]
    [Required]
    [Tooltip("Initial before explosion sprite")]
    public Sprite InitSprite;

    [FoldoutGroup("Bomb Attack Properties")]
    [Required]
    [Tooltip("Bomb explosion sprite")]
    public Sprite ExplosionSprite;
    #endregion
}