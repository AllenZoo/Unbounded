using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "AttackerData", menuName = "System/Combat/Attack", order = 1)]
public class AttackData:ScriptableObject
{
    public string attackName = "Attack";

    [Required]
    [Tooltip("Gameobject representation of attack")]
    [JsonIgnore]
    public GameObject attackPfb;

    [Tooltip("Base damage number of attack")]
    public float baseDamage = 5f;

    // TODO: remove this field.
    //[Tooltip("Base duration in seconds until attack disappears")]
    //public float duration = 0.5f;

    [Tooltip("Sound to play whenever attack is made")]
    public AudioClip attackSound;

    [Tooltip("Base distance the attack can travel. Used to calculate duration (duration = distance/speed)")]
    public float distance = 5f;

    [Tooltip("Initial Speed of Attack.")]
    public float initialSpeed = 1f;

    [Tooltip("Base nockback value of attack")]
    public float baseKnockback = 0f;

    [Tooltip("Base stun duration")]
    public float baseStunDuration = 1f;

    [Tooltip("If true, the attack will hit all targets in the collider. If false, it will only hit the first target.")]
    public bool isAOE = false;

    [Tooltip("If true, the attack will be able to hit the same target multiple times.")]
    public bool canRepeat = false;

    [Header("For DOT")]
    [Tooltip("If true, the attack will deal damage over time.")]
    public bool isDOT = false;

    [Tooltip("Duration of DOT effect in seconds.")]
    public float dotDuration = 5f;

    [Tooltip("If true, the attack will pierce through targets.")]
    public bool isPiercing = false;

    [Tooltip("If true, the attack will last until duration is over.")]
    public bool lastsUntilDuration = false;

    [Tooltip("Rotational offset for rotating attack on spawn")]
    public float rotOffset = 0f;
}
