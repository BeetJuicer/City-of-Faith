using UnityEngine;

public class Fish : MonoBehaviour
{
    private Camera mainCamera;

    public float jumpSpeed = 3f;            // Horizontal jump speed
    public float verticalOffset = 0.5f;     // Small vertical rise during jump
    public float lifeTime = 3f;             // Time before the fish disappears

    private FishAudioSource fishAudioSource; // Reference to the audio source for sound effects
    private Vector2 movementDirection;      // Direction in which the fish will move

    // Method to assign the FishAudioSource to the fish instance
    public void SetAudioSource(FishAudioSource audioSource)
    {
        fishAudioSource = audioSource;
    }

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera for checking screen bounds

        // Check if the FishAudioSource exists in the scene
        fishAudioSource = FindObjectOfType<FishAudioSource>();
        if (fishAudioSource == null)
            Debug.LogError("FishAudioSource not found in the scene!");

        // Initialize the movement direction (left or right)
        InitializeMovementDirection();

        // Destroy the fish after a set lifetime
        Invoke(nameof(DestroyObject), lifeTime);
    }

    void Update()
    {
        // Move the fish in the calculated direction
        transform.Translate(movementDirection * jumpSpeed * Time.deltaTime);

        // Check if the fish is out of the screen, and destroy it if true
        if (IsOutOfScreen())
            Destroy(gameObject);
    }

    // Method to initialize the movement direction of the fish
    private void InitializeMovementDirection()
    {
        // Randomly choose to move left or right
        float randomHorizontalDirection = Random.value < 0.5f ? -1 : 1;
        movementDirection = new Vector2(randomHorizontalDirection, verticalOffset).normalized;

        // Play the jump sound effect
        fishAudioSource?.PlayJumpSound();
    }

    // Check if the fish has moved off the screen
    private bool IsOutOfScreen()
    {
        Vector3 screenPosition = mainCamera.WorldToViewportPoint(transform.position);
        // Return true if the fish is out of bounds
        return screenPosition.x < -0.1f || screenPosition.x > 1.1f || screenPosition.y < -0.1f;
    }

    // Destroy the fish object after its lifetime expires
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    // Method that is triggered when the fish is tapped (caught)
    public void OnTap()
    {
        // Add score when the fish is tapped
        FindObjectOfType<FishGameController>().AddScore(10);

        // Play the sound effect for catching the fish
        fishAudioSource?.PlayPopSFX();

        // Destroy the fish object after being tapped
        Destroy(gameObject);
    }
}
