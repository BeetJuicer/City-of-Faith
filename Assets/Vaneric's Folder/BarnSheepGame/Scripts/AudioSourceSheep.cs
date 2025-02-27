using UnityEngine;

public class AudioSourceSheep : MonoBehaviour
{
    public static AudioSourceSheep Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip tapSound;
    public AudioClip sheepTapSound;
    public AudioClip nextWaveSound;
    public AudioClip lostSheepBaaSound;
    public AudioClip rewardSound;
    public AudioClip backgroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load saved volumes or set defaults
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        PlayBackgroundMusic();
    }
    public void PlayBackgroundMusic()
    {
        if (musicSource && backgroundMusic)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource && clip)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayTapSound()
    {
        PlaySFX(tapSound);
    }
    public void PlaySheepTapSound()
    {
        PlaySFX(sheepTapSound);
    }
    public void PlayNextWaveSound()
    {
        PlaySFX(nextWaveSound);
    }
    public void PlayLostSheepBaaSound()
    {
        PlaySFX(lostSheepBaaSound);
    }
    public void PlayRewardSound()
    {
        PlaySFX(rewardSound);
    }

    public float GetMusicVolume()
    {
        return musicSource.volume;
    }

    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume); // Save volume
    }
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume); // Save volumex`
    }
}
