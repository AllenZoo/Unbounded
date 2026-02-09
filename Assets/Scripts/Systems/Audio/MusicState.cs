/// <summary>
/// Enum defining all possible music states for map music system.
/// Allows for easy expansion with additional music modes.
/// </summary>
public enum MusicState
{
    /// <summary>
    /// No music playing
    /// </summary>
    None,
    
    /// <summary>
    /// Peaceful/ambient music during exploration
    /// </summary>
    Peaceful,
    
    /// <summary>
    /// Boss fight music when boss is aggroed
    /// </summary>
    BossFight,
    
    /// <summary>
    /// General combat music (future use)
    /// </summary>
    Combat,
    
    /// <summary>
    /// Music when player health is low (future use)
    /// </summary>
    LowHealth,
    
    /// <summary>
    /// Victory music after defeating boss (future use)
    /// </summary>
    Victory,
    
    /// <summary>
    /// Cutscene music (future use)
    /// </summary>
    Cutscene
}
