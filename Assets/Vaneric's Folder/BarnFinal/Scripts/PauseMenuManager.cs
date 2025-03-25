using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseManager;
    public GameObject soundPanel;
    public GameObject exitConfirmationPanel;

    [Header("Buttons")]
    public Button pauseButton;
    public Button resumeButton;
    public Button tutorialButton;
    public Button soundButton;
    public Button soundBackButton;
    public Button exitButton;

    [Header("Exit Confirmation Buttons")]
    public Button exitYesButton;
    public Button exitNoButton;

    [Header("Audio Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [SerializeField] private GameObject AudioManager;

    private bool isPaused = false;


    void Start()
    {
        InitializeUI();
        SetupButtonListeners();
        if (TutorialControllerBarn.Instance != null)
            TutorialControllerBarn.Instance.onTutorialClosed += OnTutorialClosed;
    }

    private void InitializeUI()
    {
        pauseManager.SetActive(false);
        soundPanel.SetActive(false);
        exitConfirmationPanel.SetActive(false);
    }

    private void SetupButtonListeners()
    {
        soundButton.onClick.AddListener(OpenSoundMenu);
        exitButton.onClick.AddListener(ShowExitConfirmation);
        exitYesButton.onClick.AddListener(ExitGame);
        exitNoButton.onClick.AddListener(CancelExit);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void PauseGame()
    {
        AudioSourceBarn.Instance.PlayTapSound();
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0; // Pause game
        pauseManager.SetActive(true);
    }

    public void ResumeGame()
    {
        AudioSourceBarn.Instance.PlayTapSound();
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1; // Resume game
        pauseManager.SetActive(false);
    }

    public void OpenSoundMenu()
    {
        AudioSourceBarn.Instance.PlayTapSound();
        pauseManager.SetActive(false);
        soundPanel.SetActive(true);
    }

    public void CloseSoundMenu()
    {
        AudioSourceBarn.Instance.PlayTapSound();
        soundPanel.SetActive(false);
        pauseManager.SetActive(true);
    }

    private void SetMusicVolume(float volume)
    {
        AudioSourceBarn.Instance?.SetMusicVolume(volume);
    }

    private void SetSFXVolume(float volume)
    {
        AudioSourceBarn.Instance?.SetSFXVolume(volume);
    }

    private void ShowExitConfirmation()
    {
        pauseManager.SetActive(false);
        exitConfirmationPanel.SetActive(true);
        AudioSourceBarn.Instance?.PlayTapSound();
    }

    private void CancelExit()
    {
        exitConfirmationPanel.SetActive(false);
        pauseManager.SetActive(true);
        AudioSourceBarn.Instance?.PlayTapSound();
    }

    public void OpenTutorial()
    {
        if (TutorialControllerBarn.Instance != null)
        {
            TutorialControllerBarn.Instance.OpenTutorial();
            pauseManager.SetActive(false);
        }
        else
        {
            Debug.LogError("TutorialFishController not found!");
        }
        AudioSourceBarn.Instance.PlayTapSound();
    }
    private void OnTutorialClosed()
    {
        pauseManager.SetActive(true);
        AudioSourceBarn.Instance?.PlayTapSound();
    }

    public void ExitGame()
    {
        Debug.Log("Exited barn mini-game and restored game state.");
    }

}
