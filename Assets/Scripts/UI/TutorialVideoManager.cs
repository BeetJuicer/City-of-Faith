using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialVideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private Image blurImage;
    [SerializeField] private VideoClip[] tutorialVideos;

    private int currentIndex = 1;

    private void Start()
    {
        if (tutorialVideos.Length > currentIndex)
        {
            Debug.Log("Starting the first tutorial video.");
            PlayTutorialVideo(0); //play first video
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseVideo);
        }
        else
        {
            Debug.LogError("No tutorial videos assigned in the tutorialVideos array.");
        }
    }

    public void TriggerTutorial()
    {
        Debug.Log($"TriggerTutorial called at index {currentIndex}");
        if (currentIndex < tutorialVideos.Length)
        {
            PlayTutorialVideo(currentIndex); //play 2nd video which is index = 1
            currentIndex++; // increment for future videos
        }
        else
        {
            Debug.LogWarning("All tutorial videos have been played.");
        }
    }

    public void PlayTutorialVideo(int index)
    {

        Debug.Log($"Video display active: {videoDisplay.gameObject.activeSelf}");
        Debug.Log($"Play Tutorial called at index {index}");
        HUDCanvas.gameObject.SetActive(false);
        blurImage.gameObject.SetActive(true);
        videoDisplay.gameObject.SetActive(true);
        closeButton.gameObject.SetActive(true);
        videoPlayer.clip = tutorialVideos[index];
        videoPlayer.enabled = true;
        videoPlayer.Prepare();
        videoPlayer.Play();
    }

    public void CloseVideo()
    {
        videoPlayer.Stop();
        HUDCanvas.gameObject.SetActive(true);
        blurImage.gameObject.SetActive(false);
        videoDisplay.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

}
