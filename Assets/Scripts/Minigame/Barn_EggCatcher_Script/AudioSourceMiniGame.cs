using UnityEngine;

public class AudioSourceMiniGame : MonoBehaviour
{
    // Singleton instance to ensure only one instance of this class exists in the game
    public static AudioSourceMiniGame instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Audio source for background music
    public AudioSource sfxSource;  // Audio source for sound effects (SFX)

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;       // Clip for the background music
    public AudioClip startSFX;              // Clip for the start sound effect
    public AudioClip eggCatchSound;         // Clip for the egg catch sound effect
    public AudioClip bombExplosionSound;    // Clip for the bomb explosion sound effect
    public AudioClip gameOverSound;         // Clip for the game-over sound effect

    [Range(0f, 1f)]
    public float bombSoundVolume = 1.0f;    // Adjustable volume for the bomb explosion sound

    [Range(0f, 1f)]
    public float backgroundMusicVolume = 0.3f; // Default volume for the background music

    void Awake()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null); // Detach from parent to make it a root GameObject
        }

        DontDestroyOnLoad(gameObject);
        Debug.Log("[AudioSourceMiniGame] GameObject set to DontDestroyOnLoad.");
    }


    // Method to start playing the background music
    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;       // Assign the background music clip
            musicSource.loop = true;                  // Enable looping for continuous playback
            musicSource.volume = backgroundMusicVolume; // Set the volume level
            musicSource.Play();                       // Start playing the music
        }
        else
        {
            Debug.LogWarning("MusicSource or BackgroundMusic is missing!"); // Log a warning if sources are missing
        }
    }

    // Method to stop the background music
    public void StopBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop(); // Stop the music if it's currently playing
        }
    }

    // Generic method to play any sound effect
    public void PlaySoundEffect(AudioClip clip, float volume = 1.0f)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume); // Play the sound effect with the specified volume
        }
        else
        {
            Debug.LogWarning("SFXSource or AudioClip is missing!"); // Log a warning if sources are missing
        }
    }

    // Method to play the start sound effect
    public void PlayStartSound()
    {
        PlaySoundEffect(startSFX);
    }

    // Method to play the game-over sound effect
    public void PlayGameOverSound()
    {
        PlaySoundEffect(gameOverSound);
    }

    // Method to play the bomb explosion sound effect
    public void PlayBombExplosionSound()
    {
        sfxSource.volume = bombSoundVolume; // Adjust the SFX source volume specifically for the bomb sound
        PlaySoundEffect(bombExplosionSound, bombSoundVolume); // Play the bomb explosion sound
    }
}
