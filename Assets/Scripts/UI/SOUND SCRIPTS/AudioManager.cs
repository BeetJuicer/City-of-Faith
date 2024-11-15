using UnityEngine;

// Manages background music and sound effects (SFX) for the game.
// Allows for easy playback of background music and one-shot sound effects.

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Source --------")]
    [SerializeField] AudioSource musicSource; // Audio source for playing background music
    [SerializeField] AudioSource SFXSource;   // Audio source for playing sound effects

    [Header("---------- Audio Clip --------")]
    public AudioClip background; // Background music clip
    public AudioClip start;      // Start sound effect clip

    // Initializes the AudioManager by starting the background music.
    private void Start()
    {
        musicSource.clip = background; // Assigns the background music to the music source
        musicSource.Play();            // Starts playing the background music
    }

    // Plays a sound effect using the SFX audio source.
 
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip); // Plays the sound effect without interrupting others
    }
}
