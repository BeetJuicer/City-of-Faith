using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseUI;
    public GameObject soundPanel;
    public GameObject exitConfirmationPanel;

    [Header("Buttons")]
    public Button pauseButton;
    public Button resumeButton;
    public Button tutorialButton;
    public Button soundButton;
    public Button closeSoundButton;
    public Button exitButton;

    [Header("Exit Confirmation Buttons")]
    public Button exitYesButton;
    public Button exitNoButton;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private bool isPaused = false;

    void Start()
    {
        pauseUI.SetActive(false);
        soundPanel.SetActive(false);
        exitConfirmationPanel.SetActive(false);

        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(ResumeGame);
        soundButton.onClick.AddListener(OpenSoundPanel);
        closeSoundButton.onClick.AddListener(CloseSoundPanel);
        exitButton.onClick.AddListener(ShowExitConfirmation);
        exitYesButton.onClick.AddListener(ExitGame);
        exitNoButton.onClick.AddListener(CancelExit);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        // Subscribe to the tutorial closed event
        if (TutorialController.Instance != null)
        {
            TutorialController.Instance.onTutorialClosed += OnTutorialClosed;
        }

        if (AudioSourceSheep.Instance != null)
        {
            musicSlider.value = AudioSourceSheep.Instance.GetMusicVolume();
            sfxSlider.value = AudioSourceSheep.Instance.GetSFXVolume();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            pauseUI.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            ResumeGame();
        }

        AudioSourceSheep.Instance?.PlayTapSound();
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseUI.SetActive(false);
        soundPanel.SetActive(false);
        exitConfirmationPanel.SetActive(false);
        Time.timeScale = 1;

        AudioSourceSheep.Instance?.PlayTapSound();
    }

    public void OpenTutorial()
    {
        if (TutorialController.Instance != null)
        {
            TutorialController.Instance.OpenTutorial();
            pauseUI.SetActive(false);
        }
        else
        {
            Debug.LogError("TutorialController not found!");
        }
    }

    private void OnTutorialClosed()
    {
        // Restore the pause menu when tutorial closes
        pauseUI.SetActive(true);
        AudioSourceSheep.Instance?.PlayTapSound();
    }

    private void OpenSoundPanel()
    {
        pauseUI.SetActive(false);
        soundPanel.SetActive(true);
        AudioSourceSheep.Instance?.PlayTapSound();
    }

    private void CloseSoundPanel()
    {
        soundPanel.SetActive(false);
        pauseUI.SetActive(true);
        AudioSourceSheep.Instance?.PlayTapSound();
    }

    private void SetMusicVolume(float volume)
    {
        AudioSourceSheep.Instance?.SetMusicVolume(volume);
    }

    private void SetSFXVolume(float volume)
    {
        AudioSourceSheep.Instance?.SetSFXVolume(volume);
    }

    private void ShowExitConfirmation()
    {
        pauseUI.SetActive(false);
        exitConfirmationPanel.SetActive(true);
        AudioSourceSheep.Instance?.PlayTapSound();
    }

    private void CancelExit()
    {
        exitConfirmationPanel.SetActive(false);
        pauseUI.SetActive(true);
        AudioSourceSheep.Instance?.PlayTapSound();
    }

    private void ExitGame()
    {
        AudioSourceSheep.Instance?.PlayTapSound();
        Debug.Log("Exiting Game...");
    }
}
