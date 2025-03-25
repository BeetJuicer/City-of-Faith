using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialControllerBarn : MonoBehaviour
{
    public static TutorialControllerBarn Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject tutorialPanel;
    public RawImage tutorialImage;
    public TextMeshProUGUI instructionText;

    [Header("Tutorial Content")]
    public Texture[] tutorialImages;
    public string[] instructions;

    [Header("Navigation Buttons")]
    public Button nextButton;
    public Button backButton;
    public Button closeButton;

    private int currentSlide = 0;
    public event System.Action onTutorialClosed;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        tutorialPanel.SetActive(false);

        nextButton.onClick.AddListener(NextSlide);
        backButton.onClick.AddListener(PreviousSlide);
        closeButton.onClick.AddListener(CloseTutorial);

        ValidateContent();
    }

    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        currentSlide = 0;
        ShowSlide();
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        onTutorialClosed?.Invoke();
    }

    private void ValidateContent()
    {
        if (tutorialImages == null || tutorialImages.Length == 0)
            Debug.LogError("Tutorial images are missing!");

        if (instructions == null || instructions.Length == 0)
            Debug.LogError("Tutorial instructions are missing!");

        if (tutorialImages.Length != instructions.Length)
            Debug.LogWarning("Image and instruction counts do not match!");
    }

    private void ShowSlide()
    {
        if (currentSlide < 0 || currentSlide >= tutorialImages.Length) return;

        tutorialImage.texture = tutorialImages[currentSlide];
        instructionText.text = instructions[currentSlide];

        backButton.interactable = currentSlide > 0;
        nextButton.interactable = currentSlide < tutorialImages.Length - 1;
    }

    public void NextSlide()
    {
        if (currentSlide < tutorialImages.Length - 1)
        {
            AudioSourceBarn.Instance?.PlayTapSound();
            currentSlide++;
            ShowSlide();
        }
    }

    public void PreviousSlide()
    {
        if (currentSlide > 0)
        {
            AudioSourceBarn.Instance?.PlayTapSound();
            currentSlide--;
            ShowSlide();
        }
    }
}
