using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Audio Manager class. Any audio effects being played must go through here.
/// </summary>
[RequiredComponents(typeof(AudioSource))]
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioClip[] audioClips;
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume)
    {

    }

}

public enum SoundType
{
    Hit,
    Damaged,
}
