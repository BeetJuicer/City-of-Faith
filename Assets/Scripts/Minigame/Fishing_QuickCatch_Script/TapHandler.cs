using UnityEngine;

public class TapHandler : MonoBehaviour
{
    private FishGameController gameController; // Reference to the game controller for score management
    private FishAudioSource fishAudioSource;  // Reference to the audio source for sound effects

    public int tapRequirement = 2; // Default number of taps required to catch the object
    private int currentTaps = 0;   // Tracks the number of times this object has been tapped

    void Start()
    {
        // Find required components in the scene
        gameController = FindObjectOfType<FishGameController>();
        fishAudioSource = FindObjectOfType<FishAudioSource>();

        // Log errors if components are missing
        if (gameController == null)
        {
            Debug.LogError("FishGameController not found in the scene! Please ensure it exists.");
        }

        if (fishAudioSource == null)
        {
            Debug.LogError("FishAudioSource not found in the scene! Please ensure it exists.");
        }

        // If the object is trash, set its tap requirement to 1
        if (IsTrash())
        {
            tapRequirement = 1;
        }
    }

    private void OnMouseDown()
    {
        // Ensure required components are present before proceeding
        if (gameController == null || fishAudioSource == null)
        {
            Debug.LogError("Required components (FishGameController or FishAudioSource) are missing!");
            return;
        }

        // Handle tap based on the object's tag
        if (IsFish())
        {
            HandleFishTap();
        }
        else if (IsTrash())
        {
            HandleTrashTap();
        }
        else
        {
            Debug.LogWarning($"Unrecognized tag: {gameObject.tag}. Tap handling skipped.");
        }
    }

    private void HandleFishTap()
    {
        currentTaps++;

        // Play sound effect for tapping a fish
        fishAudioSource.PlayTapSFX();

        Debug.Log($"{gameObject.tag} tapped {currentTaps}/{tapRequirement}");

        // Check if the tap requirement is met
        if (currentTaps >= tapRequirement)
        {
            // Play sound effect for catching the fish
            fishAudioSource.PlayPopSFX();

            // Add score for catching the fish and destroy the object
            gameController.AddScore(10);
            Destroy(gameObject);
        }
    }

    private void HandleTrashTap()
    {
        // Play sound effect for tapping trash
        fishAudioSource.PlayTrashSFX();

        Debug.Log($"{gameObject.tag} tapped! Deducting score and destroying.");

        // Deduct points for trash and destroy the object
        gameController.AddScore(-20);
        Destroy(gameObject);
    }

    private bool IsFish()
    {
        // Check if the object is a fish based on its tag
        return gameObject.CompareTag("YellowFish") ||
               gameObject.CompareTag("OrangeFish") ||
               gameObject.CompareTag("PurpleFish") ||
               gameObject.CompareTag("LargeFish");
    }

    private bool IsTrash()
    {
        // Check if the object is trash based on its tag
        return gameObject.CompareTag("Trashbag") ||
               gameObject.CompareTag("RottenBanana") ||
               gameObject.CompareTag("PlasticBottle");
    }
}
