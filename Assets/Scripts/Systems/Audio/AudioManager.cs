using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Audio Manager class. Any audio effects being played must go through here.
/// </summary>
[RequiredComponents(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
    //[SerializeField] private AudioClip[] audioClips;
    [SerializeField] private Dictionary<SoundType, List<AudioClip>> audioClips;
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
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

}

public enum SoundType
{
    None,
    Hit,
    Damaged,
    Enemy_Damaged,
    Death,
}
