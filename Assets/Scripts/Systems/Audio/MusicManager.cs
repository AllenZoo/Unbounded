using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

/// <summary>
/// Central music manager for handling map-based music states.
/// Manages smooth transitions between music tracks based on gameplay state.
/// Listens to boss fight events to automatically switch between peaceful and boss music.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicManager : Singleton<MusicManager>
{
    [Tooltip("AudioSource for playing music tracks")]
    [Required, SerializeField] private AudioSource musicAudioSource;
    
    [Tooltip("Time in seconds for fade in/out transitions")]
    [SerializeField, Range(0.1f, 5f)] private float transitionDuration = 1.5f;
    
    [Tooltip("Currently active map music configuration")]
    [ReadOnly, SerializeField] private MapMusicConfig currentMapMusic;
    
    [Tooltip("Current music state")]
    [ReadOnly, SerializeField] private MusicState currentState = MusicState.None;
    
    // Track the current transition coroutine to prevent overlapping transitions
    private Coroutine transitionCoroutine;
    
    // Store event bindings for proper cleanup
    private EventBinding<OnBossFightStartEvent> bossStartBinding;
    private EventBinding<OnBossFightEndEvent> bossEndBinding;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Get or add AudioSource component
        if (musicAudioSource == null)
        {
            musicAudioSource = GetComponent<AudioSource>();
        }
        
        // Configure AudioSource for music playback
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;
        
        // Register for boss fight events
        bossStartBinding = new EventBinding<OnBossFightStartEvent>(OnBossFightStart);
        EventBus<OnBossFightStartEvent>.Register(bossStartBinding);
        
        bossEndBinding = new EventBinding<OnBossFightEndEvent>(OnBossFightEnd);
        EventBus<OnBossFightEndEvent>.Register(bossEndBinding);
    }
    
    /// <summary>
    /// Registers a map's music configuration and optionally sets an initial state.
    /// Call this when loading a new map.
    /// </summary>
    /// <param name="config">The map music configuration to use</param>
    /// <param name="initialState">Optional initial music state to play</param>
    public void RegisterMapMusic(MapMusicConfig config, MusicState initialState = MusicState.Peaceful)
    {
        if (config == null)
        {
            Debug.LogWarning("MusicManager: Attempted to register null MapMusicConfig");
            return;
        }
        
        currentMapMusic = config;
        Debug.Log($"MusicManager: Registered music config for map '{config.MapName}'");
        
        // Set initial state if different from current
        if (initialState != MusicState.None && initialState != currentState)
        {
            SetMusicState(initialState);
        }
    }
    
    /// <summary>
    /// Sets the current music state and transitions to the appropriate track.
    /// Handles smooth fade in/out between tracks.
    /// </summary>
    /// <param name="newState">The music state to transition to</param>
    public void SetMusicState(MusicState newState)
    {
        // Don't transition if already in this state
        if (currentState == newState)
        {
            return;
        }
        
        // Check if we have a music config registered
        if (currentMapMusic == null)
        {
            Debug.LogWarning($"MusicManager: Cannot set music state to {newState} - no map music config registered");
            return;
        }
        
        // Get the track for the new state
        AudioClip newTrack = currentMapMusic.GetTrack(newState);
        
        // If transitioning to None or no track exists, fade out current music
        if (newState == MusicState.None || newTrack == null)
        {
            if (newTrack == null && newState != MusicState.None)
            {
                Debug.LogWarning($"MusicManager: No music track defined for state {newState} in map '{currentMapMusic.MapName}'");
            }
            
            StartTransition(null, newState);
            return;
        }
        
        // Transition to the new track
        StartTransition(newTrack, newState);
    }
    
    /// <summary>
    /// Starts a transition to a new music track with fade in/out.
    /// </summary>
    private void StartTransition(AudioClip newTrack, MusicState newState)
    {
        // Stop any existing transition
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        
        // Start the new transition
        transitionCoroutine = StartCoroutine(TransitionToTrack(newTrack, newState));
    }
    
    /// <summary>
    /// Coroutine that handles smooth crossfade between music tracks.
    /// </summary>
    private IEnumerator TransitionToTrack(AudioClip newTrack, MusicState newState)
    {
        float startVolume = musicAudioSource.volume;
        
        // Fade out current track
        if (musicAudioSource.isPlaying)
        {
            float fadeOutTime = 0f;
            while (fadeOutTime < transitionDuration / 2f)
            {
                fadeOutTime += Time.deltaTime;
                musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, fadeOutTime / (transitionDuration / 2f));
                yield return null;
            }
            musicAudioSource.Stop();
        }
        
        // Update state
        MusicState previousState = currentState;
        currentState = newState;
        
        Debug.Log($"MusicManager: Transitioning from {previousState} to {newState}");
        
        // If new track is null (stopping music), we're done
        if (newTrack == null)
        {
            musicAudioSource.volume = startVolume;
            transitionCoroutine = null;
            yield break;
        }
        
        // Start playing new track
        musicAudioSource.clip = newTrack;
        musicAudioSource.volume = 0f;
        musicAudioSource.Play();
        
        // Fade in new track
        float fadeInTime = 0f;
        while (fadeInTime < transitionDuration / 2f)
        {
            fadeInTime += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(0f, startVolume, fadeInTime / (transitionDuration / 2f));
            yield return null;
        }
        
        // Ensure final volume is set correctly
        musicAudioSource.volume = startVolume;
        transitionCoroutine = null;
    }
    
    /// <summary>
    /// Handler for boss fight start event - transitions to boss fight music.
    /// </summary>
    private void OnBossFightStart(OnBossFightStartEvent e)
    {
        Debug.Log($"MusicManager: Boss fight started for '{e.bossName}', switching to boss music");
        SetMusicState(MusicState.BossFight);
    }
    
    /// <summary>
    /// Handler for boss fight end event - transitions back to peaceful music.
    /// </summary>
    private void OnBossFightEnd(OnBossFightEndEvent e)
    {
        Debug.Log($"MusicManager: Boss fight ended for '{e.bossName}', switching to peaceful music");
        SetMusicState(MusicState.Peaceful);
    }
    
    /// <summary>
    /// Gets the current music state.
    /// </summary>
    public MusicState GetCurrentState()
    {
        return currentState;
    }
    
    /// <summary>
    /// Gets the currently registered map music config.
    /// </summary>
    public MapMusicConfig GetCurrentMapConfig()
    {
        return currentMapMusic;
    }
    
    protected void OnEnable()
    {
        // Re-register events in case of re-enable
        if (bossStartBinding != null)
        {
            EventBus<OnBossFightStartEvent>.Register(bossStartBinding);
        }
        if (bossEndBinding != null)
        {
            EventBus<OnBossFightEndEvent>.Register(bossEndBinding);
        }
    }
    
    protected void OnDisable()
    {
        // Unregister events using stored bindings
        if (bossStartBinding != null)
        {
            EventBus<OnBossFightStartEvent>.Unregister(bossStartBinding);
        }
        if (bossEndBinding != null)
        {
            EventBus<OnBossFightEndEvent>.Unregister(bossEndBinding);
        }
    }
    
    protected void OnDestroy()
    {
        // Clean up coroutine if still running
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        
        // Unregister events
        if (bossStartBinding != null)
        {
            EventBus<OnBossFightStartEvent>.Unregister(bossStartBinding);
        }
        if (bossEndBinding != null)
        {
            EventBus<OnBossFightEndEvent>.Unregister(bossEndBinding);
        }
    }
}
