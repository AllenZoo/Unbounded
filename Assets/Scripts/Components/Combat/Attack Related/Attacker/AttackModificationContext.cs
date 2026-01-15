using UnityEngine;

/// <summary>
/// Class that holds information on modifications relevant to Attack GameObject on spawn.
/// 
/// </summary>
public class AttackModificationContext
{
    public float Scale = 1;
    public float AttackDuration = -1; // Time before attack disappears. (-1 = never)
}
