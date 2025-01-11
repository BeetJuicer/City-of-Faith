using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingScreen;

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    private IEnumerator LoadSceneAsync(int sceneId)
    {
        // Explicitly use Unity's SceneManager to avoid conflicts
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
}
