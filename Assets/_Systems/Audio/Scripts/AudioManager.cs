using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Audio Manager class. Any audio effects being played must go through here.
/// 
/// TODO: UPGRADES
///  - for each sound instantiate a new AudioSource and play the sound on that AudioSource. This allows for multiple sounds to be played at once without cutting each other out.
///  (currently we use one AudioSource and PlayOneShot, which can cause some sounds to be cut out if played at the same time.)
///  - (pool AudioSources for optimization)
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>, IDataPersistence
{
    //[SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Dictionary<SoundType, List<AudioClip>> audioClips;
    private AudioSource audioSource;

    [Required, SerializeField] private ScriptableObjectFloat backgroundMusicVolume;
    [Required, SerializeField] private ScriptableObjectFloat soundEffectsVolume;

    [Required, SerializeField] private AudioSource backgroundMusicAudioSource;
    [Required, SerializeField] private AudioSource soundEffectsAudioSource;

    private Dictionary<Guid, Coroutine> loopingSounds = new Dictionary<Guid, Coroutine>();
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

        backgroundMusicAudioSource.volume = backgroundMusicVolume.Value/100;
        soundEffectsAudioSource.volume = soundEffectsVolume.Value/100;

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

    public static void PlaySound(AudioClip clip, float volume, bool varyPitch = true)
    {
        if (clip == null) return;

        if (varyPitch)
        {
            Instance.audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        }
        else
        {
            Instance.audioSource.pitch = 1f;
        }

        Instance.audioSource.PlayOneShot(clip, volume);
    }


    public static Guid PlaySoundLoop(AudioClip clip, float volume)
    {
        // Start sound loop and return guid to stop it later.
        Guid loopId = Guid.NewGuid();
        Coroutine loopCoroutine = Instance.StartCoroutine(Instance.SoundLoopCoroutine(clip, volume));
        Instance.loopingSounds.Add(loopId, loopCoroutine);
        return loopId;
    }

    public static void StopSoundLoop(Guid loopId)
    {
        if (!Instance.loopingSounds.TryGetValue(loopId, out var loopCoroutine))
            return;

        if (loopCoroutine != null)
        {
            Instance.StopCoroutine(loopCoroutine);
            Instance.loopingSounds.Remove(loopId);
        }
    }

    private IEnumerator SoundLoopCoroutine(AudioClip clip, float volume)
    {
        while (true)
        {
            PlaySound(clip, volume);
            yield return new WaitForSeconds(clip.length);
        }
    }


    #region Data Persistence

    private void OnEnable()
    {
        if (DataPersistenceHandler.Instance != null)
            DataPersistenceHandler.Instance.Register(this);
    }

    private void OnDisable()
    {
        if (DataPersistenceHandler.Instance != null)
            DataPersistenceHandler.Instance.Unregister(this);
    }
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

    public void ResetData() { 
        // Preserve volume settings when restting data. No need to reset on new game.
    }
    #endregion
}

public enum SoundType
{
    None,
    Hit,
    Damaged,
    Enemy_Damaged,
    Death,
    Target_Dummy_Damaged,
}
