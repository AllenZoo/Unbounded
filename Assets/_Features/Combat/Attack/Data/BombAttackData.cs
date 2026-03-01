using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
[CreateAssetMenu(fileName = "new Bomb Attack Data", menuName = "System/Combat/Attack/BombAttack", order = 1)]
public class BombAttackData : AttackData
{

    #region General Bomb Attack Fields
    [FoldoutGroup("Bomb Attack Properties")]
    [Tooltip("If true, always use the explosion radius defined in this field (explosionRadius).")]
    public bool AlwaysUseThisExplosionRadius;

    [FoldoutGroup("Bomb Attack Properties")]
    [Tooltip("Radius of explosion effect")]
    public float ExplosionRadius; // TODO: this may need to be a dynamic field.. Think about adding an option that toggles this as dynamic or static.

    [FoldoutGroup("Bomb Attack Properties")]
    [Tooltip("Time in seconds before bomb explodes (after it lands)")]
    public float FuseTime;

    #endregion

    #region VFX

    [FoldoutGroup("Bomb Attack Properties")]
    [Required]
    [Tooltip("Initial before explosion sprite")]
    public Sprite InitSprite;

    [FoldoutGroup("Bomb Attack Properties")]
    [Tooltip("The layer where the sprite will be in before it explodes. " +
        "Typically the best layer to set this to is 'Projectiles'")]
    [ValueDropdown("@DrawerUtils.GetSortingLayers()")]
    public int InitSortingLayerID;

    // Note: Unnecessary since sorting order handled by VCCM
    //[FoldoutGroup("Bomb Attack Properties")]
    //public int InitSpriteSortingOrder = 5;


    [FoldoutGroup("Bomb Attack Properties")]
    [Required]
    [Tooltip("Bomb explosion sprite")]
    public Sprite ExplosionSprite;


    [FoldoutGroup("Bomb Attack Properties")]
    [Tooltip("The layer where the sprite will be in after it explodes. " +
        "Useful to ensure that the explosion aftereffect is rendered below player/other entities. " +
        "Typically the best layer to set this to is 'Entities'")]
    [ValueDropdown("@DrawerUtils.GetSortingLayers()")]
    public int ExplosionSortingLayerID;

    // Note: Unnecessary since sorting order handled by VCCM
    //[FoldoutGroup("Bomb Attack Properties")]
    //public int ExplosionSpriteSortingOrder = 5;

    #endregion
}


