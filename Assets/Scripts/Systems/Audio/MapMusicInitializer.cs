using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Component to be placed in map scenes to automatically register map music configuration
/// and set the initial music state when the scene loads.
/// </summary>
public class MapMusicInitializer : MonoBehaviour
{
    [Tooltip("The music configuration for this map")]
    [Required, SerializeField] private MapMusicConfig mapMusicConfig;
    
    [Tooltip("Initial music state to play when the map loads")]
    [SerializeField] private MusicState initialMusicState = MusicState.Peaceful;
    
    [Tooltip("Whether to register and start music automatically on scene load")]
    [SerializeField] private bool autoInitialize = true;
    
    private void Start()
    {
        if (autoInitialize)
        {
            InitializeMapMusic();
        }
    }
    
    /// <summary>
    /// Manually initializes the map music. Can be called from other scripts or events.
    /// </summary>
    public void InitializeMapMusic()
    {
        if (mapMusicConfig == null)
        {
            Debug.LogWarning("MapMusicInitializer: No MapMusicConfig assigned!");
            return;
        }
        
        if (MusicManager.Instance == null)
        {
            Debug.LogWarning("MapMusicInitializer: MusicManager not found in scene!");
            return;
        }
        
        MusicManager.Instance.RegisterMapMusic(mapMusicConfig, initialMusicState);
        Debug.Log($"MapMusicInitializer: Initialized music for map '{mapMusicConfig.MapName}' with state {initialMusicState}");
    }
}
