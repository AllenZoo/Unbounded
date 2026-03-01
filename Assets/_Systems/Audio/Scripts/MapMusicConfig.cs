using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines music tracks for each music state on a map.
/// Maps can define their own music configuration to support different music per map.
/// </summary>
[CreateAssetMenu(fileName = "MapMusicConfig", menuName = "System/Audio/Map Music Config")]
public class MapMusicConfig : SerializedScriptableObject
{
    [Tooltip("Name of the map this music configuration is for")]
    [SerializeField] private SceneField mapName;
    
    [Tooltip("Dictionary mapping music states to their corresponding audio clips")]
    [SerializeField] private Dictionary<MusicState, AudioClip> musicTracks = new Dictionary<MusicState, AudioClip>();
    
    public string MapName => mapName;
    
    /// <summary>
    /// Gets the audio clip for a given music state.
    /// Returns null if no clip is defined for the state.
    /// </summary>
    public AudioClip GetTrack(MusicState state)
    {
        if (musicTracks.TryGetValue(state, out AudioClip clip))
        {
            return clip;
        }
        return null;
    }
    
    /// <summary>
    /// Checks if a music track is defined for the given state.
    /// </summary>
    public bool HasTrack(MusicState state)
    {
        return musicTracks.ContainsKey(state) && musicTracks[state] != null;
    }
}
