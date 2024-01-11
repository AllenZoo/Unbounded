using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores fields relevant to Attackers.
[CreateAssetMenu(fileName = "SO_Attacker", menuName = "ScriptableObjects/AttackerData", order = 1)]
public class SO_Attacker : ScriptableObject
{
    public List<EntityType> TargetTypes = new List<EntityType>();
    public GameObject attackObj;

    [Tooltip("Number of attacks to spawn when attacking.")]
    public int numAttacks = 1;

    [Tooltip("Angle offset for between each attack spawned.")]
    public float angleOffset = 0f;

    // Could consider moving this to Attack, but it might make sense for the attacker to control the 
    // cooldown of attacks.
    public float cooldown = 0.5f;
}
