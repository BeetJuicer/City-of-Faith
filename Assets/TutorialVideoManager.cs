using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialVideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private Button closeButton;
    [SerializeField] private VideoClip[] tutorialVideos;
    [SerializeField] private CentralHall centralHall;


    private void OnEnable()
    {
        if (centralHall != null)
        {
            centralHall.OnPlayerLevelUp += TriggerTutorial;
        }
    }


    private void OnDisable()
    {
        if (centralHall != null)
        {
            centralHall.OnPlayerLevelUp -= TriggerTutorial;
        }
    }
    private void Start()
    {
        PlayTutorialVideo(1);
        closeButton.onClick.AddListener(CloseVideo);
    }

    // Method to play the tutorial based on the player's level
    public void TriggerTutorial(int newLevel)
    {
        if (newLevel == 1)
        {
            PlayTutorialVideo(2);
        }
    }

    // Play the tutorial video corresponding to the current level
    public void PlayTutorialVideo(int level)
    {
        if (level > 0 && level <= tutorialVideos.Length)
        {
            videoPlayer.clip = tutorialVideos[level - 1];  // Assign the correct video clip based on the level
            videoDisplay.gameObject.SetActive(true);  // Show the RawImage panel to display the video
            closeButton.gameObject.SetActive(true);
            videoPlayer.Play();
        }
    }

    // Close the video when the button is pressed
    public void CloseVideo()
    {
        videoPlayer.Stop();  // Stop the video playback
        videoDisplay.gameObject.SetActive(false);  // Hide the RawImage panel
        closeButton.gameObject.SetActive(false);
    }
}
