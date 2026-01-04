using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.Serialization;

[CreateAssetMenu(
    fileName = "NewAttackData",
    menuName = "System/Combat/Attack",
    order = 1)]
public class AttackData : ScriptableObject
{

    #region Basic Info

    [FoldoutGroup("Basic Info")]
    [Required, InfoBox("Attack name must end with a '|' character")]
    public string AttackName = "Attack|";

    [FoldoutGroup("Basic Info")]
    [Required, JsonIgnore]
    [Tooltip("Prefab used to spawn this attack")]
    public GameObject AttackPfb;

    [FoldoutGroup("Basic Info")]
    [Min(0f)]
    public float BaseDamage = 5f;

    #endregion

    #region VFX and SFX

    [FoldoutGroup("VFX and SFX")]
    public AudioClip AttackSound;

    [FoldoutGroup("VFX and SFX")]
    [Tooltip("Rotational offset applied on spawn")]
    public float RotOffset = 0f;

    #endregion

    #region Movement

    [FoldoutGroup("Movement")]
    [Min(0f)]
    [Tooltip("Maximum distance the attack can travel")]
    public float Distance = 5f;

    [FoldoutGroup("Movement")]
    [Min(0.01f)]
    [Tooltip("Initial speed of the attack")]
    public float InitialSpeed = 1f;

    [FoldoutGroup("Movement")]
    [ShowInInspector, ReadOnly]
    [LabelText("Duration (Distance / Speed) [Seconds]")]
    public float Duration => Distance / InitialSpeed;

    #endregion

    #region Combat Effects

    [FoldoutGroup("Combat Effects")]
    [Min(0f)]
    public float BaseKnockback = 0f;

    [FoldoutGroup("Combat Effects")]
    [Min(0f)]
    public float BaseStunDuration = 1f;

    [FoldoutGroup("Combat Effects")]
    public bool IsAOE = false;
    
    [FoldoutGroup("Combat Effects")]
    public bool IsPiercing = false;

    #endregion

    #region DOT

    [FoldoutGroup("Damage Over Time")]
    public bool IsDOT = false;

    [FoldoutGroup("Damage Over Time")]
    [ShowIf(nameof(IsDOT))]
    [Min(0f)]
    public float DotDuration = 5f;

    #endregion

    #region Hit Behaviour

    [FoldoutGroup("Hit Behavior")]
    public bool LastsUntilDuration = false; // TODO: check out what this field even does.

    [FoldoutGroup("Hit Behavior")]
    public bool DisappearOnHit = true;

    [FoldoutGroup("Hit Behavior")]
    [HideIf(nameof(DisappearOnHit))]
    public bool CanRepeat = false;

    [FoldoutGroup("Hit Behavior")]
    [ShowIf("@this.CanRepeat && !this.DisappearOnHit")]
    [Tooltip("Cooldown before the same target can be hit again")]
    [Min(0f)]
    public float RehitCooldown = 0f;

    #endregion
}


// TO use go to Tools/Migrate/Resave AttackData
// Note: This function saves any AttackData subclasses
#if UNITY_EDITOR
public static class AttackDataMigration
{
    [MenuItem("Tools/Migrate/Resave AttackData")]
    public static void ResaveAllAttackData()
    {
        string[] guids = AssetDatabase.FindAssets("t:AttackData");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AttackData asset = AssetDatabase.LoadAssetAtPath<AttackData>(path);

            if (asset == null)
                continue;

            EditorUtility.SetDirty(asset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("AttackData migration complete.");
    }
}
#endif

