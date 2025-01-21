using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using static Cinemachine.DocumentationSortingAttribute;

public class TutorialVideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private Image blurImage;
    [SerializeField] private Image TextBg;
    [SerializeField] private Tutorial_SO[] tutorialSO;
    [SerializeField] private CentralHall centralHall;
    [SerializeField] private TMP_Text tutorialText;

    private int currentStep = 0;
    private Tutorial_SO currentTutorial;



    private void Start()
    {
        if (centralHall.Level == 1)
        {
            currentTutorial = tutorialSO[centralHall.Level - 1];
            PlayTutorialVideo(currentTutorial);
            nextButton.onClick.AddListener(NextStep);
            prevButton.onClick.AddListener(PreviousStep);
            closeButton.gameObject.SetActive(false);

        }
        else
        {
            CloseVideo();
        }
    }
    public void TriggerTutorial()
    {

        if (centralHall.Level - 1 >= 0 && centralHall.Level - 1 < tutorialSO.Length)
        {
            currentTutorial = tutorialSO[centralHall.Level - 1];
            currentStep = 0;
            PlayTutorialVideo(currentTutorial); //play 2nd video which is index = 1
        }
        else
        {
            Debug.LogWarning("All tutorial videos have been played.");
        }
    }

    public void PlayTutorialVideo(Tutorial_SO tutorialSO)
    {
        if (tutorialSO.tutorialVideos.Length > 0)
        {
            HUDCanvas.gameObject.SetActive(false);
            blurImage.gameObject.SetActive(true);
            TextBg.gameObject.SetActive(true);
            videoDisplay.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            prevButton.gameObject.SetActive(true);
            videoPlayer.clip = tutorialSO.tutorialVideos[currentStep];
            videoPlayer.enabled = true;
            videoPlayer.isLooping = true;
            videoPlayer.Prepare();
            videoPlayer.Play();
            tutorialText.text = tutorialSO.tutorialTexts[currentStep];
        }
    }
    public void NextStep()
    {
        Debug.Log("NextStep called");
        Debug.Log("currentTutorial" + currentTutorial.name);
        Debug.Log("tut vid" + currentTutorial.tutorialVideos);

        if (currentStep < currentTutorial.tutorialVideos.Length - 1)
        {
            currentStep++;
            prevButton.interactable = true;
            videoPlayer.clip = currentTutorial.tutorialVideos[currentStep];
            videoPlayer.Play();
            tutorialText.text = currentTutorial.tutorialTexts[currentStep];

        }
        else
        {
            Debug.Log("Tutorial completed!");
            nextButton.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(true);
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseVideo);
        }
    }
    private void PreviousStep()
    {
        currentStep--;
        if (currentStep < 0)
        {
            currentStep = 0;
            return; // Prevent further execution if already at the first step
        }

        // Play the previous video and update the tutorial text
        videoPlayer.clip = currentTutorial.tutorialVideos[currentStep];
        videoPlayer.Play();
        tutorialText.text = currentTutorial.tutorialTexts[currentStep];

        // Re-enable the "Next" button if it's disabled
        nextButton.gameObject.SetActive(true);

        // Disable "Previous" button if we're at the first step
        if (currentStep == 0)
        {
            prevButton.interactable = false;
        }
    }

    public void CloseVideo()
    {
        videoPlayer.Stop();
        HUDCanvas.gameObject.SetActive(true);
        blurImage.gameObject.SetActive(false);
        videoDisplay.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        TextBg.gameObject.SetActive(false);
    }

}
