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
    [SerializeField] private CentralHall centralHall;




    private void Start()
    {
        if (centralHall.Level == 1 && tutorialVideos.Length > 0)
        {
            PlayTutorialVideo(0);
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseVideo);
        }
        else
        {
            CloseVideo();
        }
    }
    public void TriggerTutorial()
    {

        if (centralHall.Level < tutorialVideos.Length)
        {
            PlayTutorialVideo(centralHall.Level - 1); //play 2nd video which is index = 1
        }
        else
        {
            Debug.LogWarning("All tutorial videos have been played.");
        }
    }

    public void PlayTutorialVideo(int index)
    {

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
        blurImage.gameObject.SetActive(false);
        videoDisplay.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
    }

}
