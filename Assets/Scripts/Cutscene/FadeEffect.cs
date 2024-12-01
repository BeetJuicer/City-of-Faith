using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// This script provides a fade-in and fade-out effect for a UI image, typically used for scene transitions or effects.
public class FadeEffect : MonoBehaviour
{
    // Reference to the UI Image component used for the fade effect.
    // This should be a full-screen black image added to the Canvas.
    public Image fadeImage;

    // Duration of the fade effect in seconds.
    public float fadeDuration = 1f;

    private void Start()
    {
        // Initialize the image to be fully black at the start (optional, depending on usage).
        Color color = fadeImage.color;
        color.a = 1f; // Set the alpha to fully opaque.
        fadeImage.color = color;
    }

    // Initiates a fade-in effect (black screen to transparent).
    public void FadeIn()
    {
        fadeImage.gameObject.SetActive(true); // Ensure the fade image is visible.
        StartCoroutine(Fade(1f, 0f)); // Start fading from black (alpha = 1) to transparent (alpha = 0).
    }

    // Initiates a fade-out effect (transparent to black screen).
    public void FadeOut()
    {
        fadeImage.gameObject.SetActive(true); // Ensure the fade image is visible.
        StartCoroutine(Fade(0f, 1f)); // Start fading from transparent (alpha = 0) to black (alpha = 1).
    }

    // Coroutine to gradually change the alpha value of the fade image over time.
    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f; // Tracks the time elapsed since the fade started.
        Color color = fadeImage.color; // Get the current color of the fade image.

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime; // Increment elapsed time by the time since the last frame.
            // Interpolate the alpha value based on the elapsed time.
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = color; // Apply the new color to the fade image.
            yield return null; // Wait for the next frame before continuing.
        }

        // Ensure the final alpha value is set accurately (important for rounding errors).
        color.a = endAlpha;
        fadeImage.color = color;

        // If the fade ends in full transparency, deactivate the image to optimize UI rendering.
        if (endAlpha == 0f)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }
}
