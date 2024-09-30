using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class VolumeManager : MonoBehaviour
{
    public AudioMixer _audio;

    public GameObject ON;

    public GameObject OFF;

    public void SetVolume(float vol)
    {
        _audio.SetFloat("vol", vol);
    }

    public void On()
    {
        AudioListener.volume = 0;
        ON.SetActive(false);
        OFF.SetActive(true);
    }

    public void Off()
    {
        AudioListener.volume = 1;
        ON.SetActive(false);
        OFF.SetActive(true);
    }
}
