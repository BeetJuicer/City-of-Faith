using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // Required for working with UI elements

public class DelayedActivationWithText : MonoBehaviour
{
    public GameObject objectToActivate;
    public GameObject objectToDeactivate; // Assign the object in the Inspector
    public TMP_Text countdownText;          // Assign the UI Text element in the Inspector (or use TextMeshPro if preferred)
    public float delayInSeconds = 5f;   // Set the delay time in seconds in the Inspector

    void Start()
    {
        // Start the coroutine to activate the object after the specified delay and update the countdown text
        StartCoroutine(ActivateAfterDelay());
    }

    private IEnumerator ActivateAfterDelay()
    {
        // Loop through each second, updating the countdown text
        for (float i = delayInSeconds; i > 0; i--)
        {
            // Update the text to show the remaining time
            if (i > 1)
            {
                countdownText.text = "" + i.ToString("0") + " seconds ";
            }
            else
            {
                countdownText.text = "" + i.ToString("0") + " second ";
            }

            // Wait for 1 second
            yield return new WaitForSeconds(1f);
        }

        // Final message before activation (0 seconds remaining)
        countdownText.text = "0 Second";

        // Activate the object after the delay
        objectToActivate.SetActive(true);
        objectToDeactivate.SetActive(false);

        // Display confirmation message
        Debug.Log("Object activated after " + delayInSeconds + " seconds.");
    }
}
