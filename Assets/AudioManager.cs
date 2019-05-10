using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{
    private AudioSource audioSource;

    public AudioClip[] popClips;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Play the sound of the circle spawning (the pop sound).
    public void PlayPopSound()
    {
        AudioClip randomClip = popClips[Random.Range(0, popClips.Length - 1)];
        audioSource.clip = randomClip;
        audioSource.Play();
    }
}
