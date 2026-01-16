using UnityEngine;

/// <summary>
/// Class that holds information on modifications relevant to Attack GameObject on spawn.
/// Also contains information produced by Attackers
/// </summary>
public class AttackModificationContext
{
    public float ObjectScale = 1; // For scaling the sprite/gameobject of the attack
    public float AttackDuration = -1; // Time before attack disappears. (-1 = never)
    public float AngleOffset = 0; // Offset Angle to fire the attack
    public Vector3 AttackDirection = Vector3.zero; // Direction to launch the attack in (overrides if set)
}
