using UnityEngine;

public class AudioSourceFish : MonoBehaviour
{
    public static AudioSourceFish Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip tapSound;
    [SerializeField] private AudioClip rewardSound;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip messageSound;
    [SerializeField] private AudioClip fishCatchSound;

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
    public void PlayMessageSound()
    {
        PlaySFX(messageSound);
    }
    public void PlayFishCatchSound()
    {
        PlaySFX(fishCatchSound);
    }
    public void PlayRewardSound()
    {
        PlaySFX(rewardSound);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
    public float GetMusicVolume()
    {
        return musicSource.volume;
    }

    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }
}
