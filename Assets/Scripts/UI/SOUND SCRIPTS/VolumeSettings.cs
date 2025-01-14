using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

// Manages audio volume settings for music and sound effects (SFX).
// Provides functionality to save and load volume preferences using Unity's PlayerPrefs.

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer; // Reference to the Audio Mixer to control audio groups
    [SerializeField] private Slider musicSlider; // Slider for adjusting music volume
    [SerializeField] private Slider SFXSlider;   // Slider for adjusting SFX volume

    // Initializes volume settings on start.
    // Loads saved volume preferences or sets defaults if no preferences are found.
    private void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume(); // Load previously saved volume settings
        }
        else
        {
            SetMusicVolume(); // Set default music volume
            SetSFXVolume();   // Set default SFX volume
        }
    }

    // Updates the music volume based on the music slider's value.
    // Saves the volume setting to PlayerPrefs.
    public void SetMusicVolume()
    {
        float volume = musicSlider.value; // Get slider value
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20); // Convert to logarithmic scale for decibels
        PlayerPrefs.SetFloat("musicVolume", volume); // Save volume to PlayerPrefs
    }

    // Updates the SFX volume based on the SFX slider's value.
    // Saves the volume setting to PlayerPrefs.
    public void SetSFXVolume()
    {
        float volume = SFXSlider.value; // Get slider value
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20); // Convert to logarithmic scale for decibels
        PlayerPrefs.SetFloat("SFXVolume", volume); // Save volume to PlayerPrefs
    }

    // Loads saved volume settings from PlayerPrefs and applies them to sliders and the Audio Mixer.

    private void LoadVolume()
    {
        // Retrieve saved volume settings
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");

        // Apply loaded settings
        SetMusicVolume();
        SetSFXVolume();
    }
}
