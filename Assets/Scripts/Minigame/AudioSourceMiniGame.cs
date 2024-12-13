using UnityEngine;

public class AudioSourceMiniGame : MonoBehaviour
{
    public static AudioSourceMiniGame instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Dedicated for background music
    public AudioSource sfxSource;  // Dedicated for sound effects

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip startSFX;
    public AudioClip eggCatchSound;
    public AudioClip bombExplosionSound;
    public AudioClip gameOverSound;

    [Range(0f, 1f)]
    public float bombSoundVolume = 1.0f; // Adjustable bomb sound volume

    [Range(0f, 1f)]
    public float backgroundMusicVolume = 0.3f; // Default volume for background music

    void Awake()
    {
        // Singleton Pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = backgroundMusicVolume; // Set background music volume
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("MusicSource or BackgroundMusic is missing!");
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PlaySoundEffect(AudioClip clip, float volume = 1.0f)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume); // Play SFX at the specified volume
        }
        else
        {
            Debug.LogWarning("SFXSource or AudioClip is missing!");
        }
    }

    public void PlayStartSound()
    {
        PlaySoundEffect(startSFX);
    }

    public void PlayGameOverSound()
    {
        PlaySoundEffect(gameOverSound);
    }

    public void PlayBombExplosionSound()
    {
        // Ensure bomb explosion sound is played at the correct volume
        sfxSource.volume = bombSoundVolume; // Set bomb explosion volume
        PlaySoundEffect(bombExplosionSound, bombSoundVolume);
    }
}
