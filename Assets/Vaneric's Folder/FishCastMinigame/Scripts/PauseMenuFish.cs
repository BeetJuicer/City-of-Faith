using UnityEngine;
using UnityEngine.UI;

public class PauseMenuFish : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject soundMenuUI;
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

    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private Camera MainCamera3d;
    [SerializeField] private GameObject fishingMinigamePrefab;
    [SerializeField] private GameObject AudioManager;

    private bool isPaused = false;

    void Start()
    {
        InitializeUI();
        SetupButtonListeners();
        if (TutorialFishController.Instance != null)
            TutorialFishController.Instance.onTutorialClosed += OnTutorialClosed;

    }

    private void InitializeUI()
    {
        pauseMenuUI.SetActive(false);
        soundMenuUI.SetActive(false);
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

    private void InitializeAudioSliders()
    {
        if (AudioSourceFish.Instance == null) return;

        musicSlider.value = AudioSourceFish.Instance.GetMusicVolume();
        sfxSlider.value = AudioSourceFish.Instance.GetSFXVolume();
    }

    public void PauseGame()
    {
        AudioSourceFish.Instance.PlayTapSound();
        if (isPaused) return;
        isPaused = true;
        Time.timeScale = 0; // Pause game
        pauseMenuUI.SetActive(true);
    }

    public void ResumeGame()
    {
        AudioSourceFish.Instance.PlayTapSound();
        if (!isPaused) return;
        isPaused = false;
        Time.timeScale = 1; // Resume game
        pauseMenuUI.SetActive(false);
    }

    public void OpenSoundMenu()
    {
        AudioSourceFish.Instance.PlayTapSound();
        pauseMenuUI.SetActive(false);
        soundMenuUI.SetActive(true);
    }

    public void CloseSoundMenu()
    {
        AudioSourceFish.Instance.PlayTapSound();
        soundMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    private void SetMusicVolume(float volume)
    {
        AudioSourceFish.Instance?.SetMusicVolume(volume);
    }

    private void SetSFXVolume(float volume)
    {
        AudioSourceFish.Instance?.SetSFXVolume(volume);
    }

    private void ShowExitConfirmation()
    {
        pauseMenuUI.SetActive(false);
        exitConfirmationPanel.SetActive(true);
        AudioSourceFish.Instance?.PlayTapSound();
    }

    private void CancelExit()
    {
        exitConfirmationPanel.SetActive(false);
        pauseMenuUI.SetActive(true);
        AudioSourceFish.Instance?.PlayTapSound();
    }

    public void OpenTutorial()
    {
        if (TutorialFishController.Instance != null)
        {
            TutorialFishController.Instance.OpenTutorial();
            pauseMenuUI.SetActive(false);
        }
        else
        {
            Debug.LogError("TutorialFishController not found!");
        }
    }
    private void OnTutorialClosed()
    {
        pauseMenuUI.SetActive(true);
        AudioSourceFish.Instance?.PlayTapSound();
    }
    public void ExitGame()
    {
        Time.timeScale = 1f;
        FishingController.Instance?.ResetGameState();

        pauseMenuUI.SetActive(false);
        soundMenuUI.SetActive(false);
        exitConfirmationPanel.SetActive(false);
        isPaused = false;

        // Ensure the fishing mini-game is deactivated properly
        if (fishingMinigamePrefab != null)
            fishingMinigamePrefab.SetActive(false);

        // Restore HUD and camera visibility
        if (HUDCanvas != null)
            HUDCanvas.SetActive(true);

        if (MainCamera3d != null)
            MainCamera3d.gameObject.SetActive(true);

        // Ensure audio manager remains active
        if (AudioManager != null)
            AudioManager.gameObject.SetActive(true);

        Debug.Log("Exited fishing mini-game and restored game state.");
    }

}
