using UnityEngine;

public class FishAudioSource : MonoBehaviour
{
    // Audio sources for playing music and sound effects
    public AudioSource musicSource; // Source for background music
    public AudioSource sfxSource;   // Source for sound effects

    // Audio clips for various game sounds
    public AudioClip backgroundMusic; // Background music track
    public AudioClip startSFX;        // Sound effect for game start
    public AudioClip popSFX;          // Sound effect for catching a fish
    public AudioClip tapSFX;          // Sound effect for tapping a fish
    public AudioClip trashSFX;        // Sound effect for tapping trash
    public AudioClip gameOverSFX;     // Sound effect for game over
    public AudioClip jumpSFX;         // Sound effect for fish jumping

    // Volume controls for specific sound effects
    public float trashSFXVolume = 1f;          // Volume for trash sound effects
    public float backgroundMusicVolume = 0.353f; // Volume for background music

    // Play the sound effect for starting the game
    public void PlayStartSFX()
    {
        sfxSource.PlayOneShot(startSFX);
    }

    // Play the sound effect for catching a fish
    public void PlayPopSFX()
    {
        sfxSource.PlayOneShot(popSFX);
    }

    // Play the sound effect for tapping a fish
    public void PlayTapSFX()
    {
        sfxSource.PlayOneShot(tapSFX);
    }

    // Play the sound effect for tapping trash, with adjustable volume
    public void PlayTrashSFX()
    {
        sfxSource.PlayOneShot(trashSFX, trashSFXVolume);
    }

    // Play the sound effect for game over
    public void PlayGameOverSFX()
    {
        sfxSource.PlayOneShot(gameOverSFX);
    }

    // Start playing the background music
    public void PlayBackgroundMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.volume = backgroundMusicVolume;
        musicSource.Play();
    }

    // Stop the background music
    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }

    // Play the sound effect for fish jumping
    public void PlayJumpSound()
    {
        sfxSource.PlayOneShot(jumpSFX);
    }
}
