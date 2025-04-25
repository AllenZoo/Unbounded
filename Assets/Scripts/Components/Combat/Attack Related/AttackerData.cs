using UnityEngine;

/// <summary>
/// Class that holds AttackerData. It is essentially an attack pattern.
/// </summary>
[CreateAssetMenu(fileName = "AttackerData", menuName = "System/Combat/Attacker", order = 1)]
public class AttackerData : ScriptableObject
{
    [Tooltip("Number of attacks to spawn when attacking.")]
    public int numAttacks = 1;

    [Tooltip("Angle offset for between each attack spawned.")]
    public float angleOffset = 0f;

    [Tooltip("Cooldown of attacker to launch attacks.")]
    public float cooldown = 0.5f;

    [Tooltip("time it takes to charge up attack.")]
    public float chargeUp = 0f;

    [SerializeField]
    [TextArea(3, 10)]
    [Tooltip("description for designer use")]
    private string description;
}
