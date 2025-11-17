using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Audio Manager class. Any audio effects being played must go through here.
/// </summary>
[RequiredComponents(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>, IDataPersistence
{
    //[SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Dictionary<SoundType, List<AudioClip>> audioClips;
    private AudioSource audioSource;

    [Required, SerializeField] private ScriptableObjectFloat backgroundMusicVolume;
    [Required, SerializeField] private ScriptableObjectFloat soundEffectsVolume;

    [Required, SerializeField] private AudioSource backgroundMusicAudioSource;
    [Required, SerializeField] private AudioSource soundEffectsAudioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    protected void Start()
    {
        if (backgroundMusicVolume == null ||  backgroundMusicAudioSource == null || backgroundMusicAudioSource == null || soundEffectsAudioSource == null)
        {
            Debug.LogError("Some fields were not initialized/dragged in properly!");
            return;
        }

        backgroundMusicVolume.OnValueChanged += (float volume) => backgroundMusicAudioSource.volume = volume/100;
        soundEffectsVolume.OnValueChanged += (float volume) => soundEffectsAudioSource.volume = volume/100;
    } 

    public static void PlaySound(SoundType sound, float volume)
    {
        if (!Instance.audioClips.ContainsKey(sound)) return;

        var audioClips = Instance.audioClips[sound];

        if (audioClips == null) return;

        foreach (var clip in audioClips)
        {
            Instance.audioSource.PlayOneShot(clip, volume);
        }
    }

    public static void PlaySound(AudioClip clip, float volume)
    {
        if (clip == null) return;

        Instance.audioSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Load Audio Data
    /// </summary>
    /// <param name="data"></param>
    public void LoadData(GameData data)
    {
        backgroundMusicVolume.Set(data.backgroundMusicVolume);
        soundEffectsVolume.Set(data.soundEffectsVolume);
    }

    /// <summary>
    /// Save Audio Data
    /// </summary>
    /// <param name="data"></param>
    public void SaveData(GameData data)
    {
        data.backgroundMusicVolume = backgroundMusicVolume.Value;
        data.soundEffectsVolume = soundEffectsVolume.Value;
    }
}

public enum SoundType
{
    None,
    Hit,
    Damaged,
    Enemy_Damaged,
    Death,
}
