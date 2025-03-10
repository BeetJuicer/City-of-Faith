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

    [Header("Scene References")]
    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private Camera MainCamera3d;
    [SerializeField] private GameObject barnMinigamePrefab;

    private bool isPaused = false;

    private void Start()
    {
        InitializeUI();
        SetupButtonListeners();
        InitializeAudioSliders();

        if (TutorialController.Instance != null)
            TutorialController.Instance.onTutorialClosed += OnTutorialClosed;

        SetSheepVisibility(true);
    }

    private void InitializeUI()
    {
        pauseUI.SetActive(false);
        soundPanel.SetActive(false);
        exitConfirmationPanel.SetActive(false);
    }

    private void SetupButtonListeners()
    {
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(ResumeGame);
        tutorialButton.onClick.AddListener(OpenTutorial);
        soundButton.onClick.AddListener(OpenSoundPanel);
        closeSoundButton.onClick.AddListener(CloseSoundPanel);
        exitButton.onClick.AddListener(ShowExitConfirmation);
        exitYesButton.onClick.AddListener(ExitGame);
        exitNoButton.onClick.AddListener(CancelExit);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void InitializeAudioSliders()
    {
        if (AudioSourceSheep.Instance == null) return;

        musicSlider.value = AudioSourceSheep.Instance.GetMusicVolume();
        sfxSlider.value = AudioSourceSheep.Instance.GetSFXVolume();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }

        AudioSourceSheep.Instance?.PlayTapSound();
    }

    private void PauseGame()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0;
        SetSheepVisibility(false);
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseUI.SetActive(false);
        soundPanel.SetActive(false);
        exitConfirmationPanel.SetActive(false);

        Time.timeScale = 1;
        SetSheepVisibility(true); // Ensure sheep are visible when resuming

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
        pauseUI.SetActive(true);
        if (!isPaused) // Only restore sheep visibility if the game is NOT paused
            SetSheepVisibility(true);
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

        if (!isPaused) // Only restore sheep visibility if the game is NOT paused
            SetSheepVisibility(true);

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

        if (!isPaused) // Only restore sheep visibility if the game is NOT paused
            SetSheepVisibility(true);

        AudioSourceSheep.Instance?.PlayTapSound();
    }


    public void ExitGame()
    {
        Time.timeScale = 1f;
        GameControllerSheep.Instance?.ResetGameState();

        pauseUI.SetActive(false);
        soundPanel.SetActive(false);
        exitConfirmationPanel.SetActive(false);
        isPaused = false;

        if (barnMinigamePrefab != null)
            barnMinigamePrefab.SetActive(false);

        if (HUDCanvas != null)
            HUDCanvas.SetActive(true);

        if (MainCamera3d != null)
            MainCamera3d.gameObject.SetActive(true);

        SetSheepVisibility(true);
    }

    private void SetSheepVisibility(bool isVisible)
    {
        GameObject[] sheepObjects = GameObject.FindGameObjectsWithTag("Sheep");

        foreach (GameObject sheep in sheepObjects)
        {
            if (sheep != null)
            {
                // Instead of SetActive, only disable the renderer
                SpriteRenderer renderer = sheep.GetComponent<SpriteRenderer>();
                if (renderer != null) renderer.enabled = isVisible;

                // Optional: Reactivate sheep movement if needed
                Rigidbody2D rb = sheep.GetComponent<Rigidbody2D>();
                if (rb != null) rb.simulated = isVisible;
            }
        }
    }


}