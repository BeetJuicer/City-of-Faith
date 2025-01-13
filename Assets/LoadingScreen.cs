using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingScreen; // Reference to the loading screen UI
    public GameObject cutsceneCanvas; // Reference to the GameObject containing the CutSceneCanvas
    public int targetSceneId;         // Scene to load after the cutscene
    public Database database;         // Reference to your Database script

    private VideoPlayer videoPlayer;
    private bool videoFinished = false;

    private void Start()
    {
        // Find and assign the VideoPlayer component
        if (cutsceneCanvas != null)
        {
            Transform videoPlayerTransform = cutsceneCanvas.transform.Find("CutSceneVideoPlayer");
            if (videoPlayerTransform != null)
            {
                videoPlayer = videoPlayerTransform.GetComponent<VideoPlayer>();
                if (videoPlayer != null)
                {
                    videoPlayer.loopPointReached += OnVideoFinished;
                }
                else
                {
                    Debug.LogError("No VideoPlayer component found on the CutSceneVideoPlayer object!");
                }
            }
            else
            {
                Debug.LogError("CutSceneVideoPlayer child not found under CutSceneCanvas!");
            }
        }
        else
        {
            Debug.LogError("Cutscene Canvas GameObject is not assigned!");
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.Stop(); // Stop the video player explicitly
        }

        StopAllCoroutines(); // Stop all running coroutines
    }

    public void StartGame()
    {
        var currentPlayer = database.CurrentPlayerData;

        if (currentPlayer == null)
        {
            Debug.LogError("Current player data is missing!");
            return;
        }

        if (!currentPlayer.HasSeenCutscene)
        {
            // New player: Play the cutscene and update the database
            Debug.Log("New player detected. Playing cutscene...");
            PlayCutsceneAndLoadScene(() =>
            {
                currentPlayer.HasSeenCutscene = true;
                database.UpdateRecord(currentPlayer); // Update the flag in the database
            });
        }
        else
        {
            // Old player: Skip the cutscene and directly load the scene
            Debug.Log("Old player detected. Skipping cutscene...");
            StartCoroutine(LoadSceneAsync(targetSceneId));
        }
    }

    private void PlayCutsceneAndLoadScene(Action onCutsceneComplete = null)
    {
        if (videoPlayer != null)
        {
            cutsceneCanvas.SetActive(true); // Show the cutscene GameObject
            videoPlayer.Play();
            StartCoroutine(WaitForVideoThenLoadScene(onCutsceneComplete));
        }
        else
        {
            Debug.LogError("VideoPlayer is not assigned or missing from the CutSceneVideoPlayer object!");
        }
    }

    private IEnumerator WaitForVideoThenLoadScene(Action onCutsceneComplete)
    {
        // Wait until the video finishes
        while (!videoFinished)
        {
            yield return null;
        }

        // Execute the callback
        onCutsceneComplete?.Invoke();

        // Load the next scene after the video is finished
        StartCoroutine(LoadSceneAsync(targetSceneId));
    }

    public IEnumerator LoadSceneAsync(int sceneId)
    {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneId);

        // Show the loading screen
        loadingScreen.SetActive(true);

        // Wait until the scene is fully loaded
        while (!operation.isDone)
        {
            yield return null;
        }

        // Optionally hide the loading screen after the scene is loaded
        loadingScreen.SetActive(false);
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        videoFinished = true;
        cutsceneCanvas.SetActive(false); // Optionally hide the cutscene Canvas after the video finishes
    }
}
