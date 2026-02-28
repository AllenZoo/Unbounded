using Sirenix.OdinInspector;
using UnityEngine;

public class BackgroundMusicSetter: MonoBehaviour
{
    [Required, SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundAudio;
    [SerializeField] private float audioPitch = 1.0f;
    [SerializeField] private bool setOnLoad = true;

    private void Start()
    {
        if (setOnLoad)
        {
            SetBackgroundMusic();
        }
    }

    private void SetBackgroundMusic()
    {
        if (backgroundAudio != null)
        {
            audioSource.clip = backgroundAudio;
            audioSource.loop = true;
            audioSource.pitch = audioPitch;
            audioSource.Play();
        }
    }
}
